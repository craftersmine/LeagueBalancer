using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using craftersmine.League.CommunityDragon;
using craftersmine.Riot.Api.League.Mastery;
using craftersmine.Riot.Api.League.SummonerLeagues;

namespace craftersmine.LeagueBalancer
{
    public class Balancer
    {
        private const double AllChampsDeltaWeight = 0.05d;
        private const double PlayerMainWeight = 0.6d;
        private const double PlayerMasteryChampionWeightModifier = 0.95d;

        public static Dictionary<LeagueTeamType, LeagueTeam> BalanceTeams(Summoner[] summoners)
        {
            List<Summoner> blueTeam = new List<Summoner>();
            List<Summoner> redTeam = new List<Summoner>();

            int blueTeamLp = 0;
            int redTeamLp = 0;

            int blueTeamLevel = 0;
            int redTeamLevel = 0;

            List<Summoner> orderedSummoners = summoners.OrderBy(s => s.LeaguePointsAmount).ToList();
            List<Summoner> unrankedSummoners =
                orderedSummoners.Where(s => s.SummonerLeague is null || s.SummonerLeague?.Tier == LeagueRankedTier.Unranked).OrderBy(s => s.SummonerInfo.SummonerLevel).ToList();
            orderedSummoners.RemoveAll(s => s.SummonerLeague is null || s.SummonerLeague?.Tier == LeagueRankedTier.Unranked);

            while (orderedSummoners.Any())
            {
                Summoner summoner = orderedSummoners.MaxBy(s => s.LeaguePointsAmount)!;

                if (blueTeamLp < redTeamLp && blueTeam.Count < 5)
                {
                    blueTeam.Add(summoner);
                    orderedSummoners.Remove(summoner);
                    blueTeamLp += summoner.LeaguePointsAmount;
                }
                else
                {
                    redTeam.Add(summoner);
                    orderedSummoners.Remove(summoner);
                    redTeamLp += summoner.LeaguePointsAmount;
                }
            }

            while (unrankedSummoners.Any())
            {
                Summoner summoner = unrankedSummoners.MaxBy(s => s.SummonerInfo.SummonerLevel);

                if (blueTeamLevel <= redTeamLevel && blueTeam.Count < 5)
                {
                    blueTeam.Add(summoner);
                    unrankedSummoners.Remove(summoner);
                    blueTeamLevel += (int)summoner.SummonerInfo.SummonerLevel;
                }
                else
                {
                    redTeam.Add(summoner);
                    unrankedSummoners.Remove(summoner);
                    redTeamLevel += (int)summoner.SummonerInfo.SummonerLevel;
                }
            }


            Dictionary<LeagueTeamType, LeagueTeam> teams = new Dictionary<LeagueTeamType, LeagueTeam>
            {
                { LeagueTeamType.Blue, new LeagueTeam(blueTeam.ToArray()) },
                { LeagueTeamType.Red, new LeagueTeam(redTeam.ToArray()) }
            };
            return teams;
        }

        public static async Task<LeagueChampion[]> GetChampionList(Summoner summoner, int amount, double masteryModifier = 1d)
        {
            if (AppCache.Instance.Champions is null || !AppCache.Instance.Champions.Any())
                AppCache.Instance.Champions = await App.CommunityDragonClient.GetChampionsAsync();

            LeagueChampionMastery[] masteries =
                await App.MasteryApiClient.GetMasteriesBySummonerId(summoner.Region.Region, summoner.SummonerInfo.Id);

            LeagueChampionMastery maxMastery = masteries.MaxBy(m => m.MasteryPoints)!;

            Dictionary<int, double> championWeights = new Dictionary<int, double>();
            List<int> championsWithoutMastery = new List<int>((AppCache.Instance.Champions.Count - 1) - masteries.Length);
            double otherChampsProbability = 1d - (0d / maxMastery.MasteryPoints) - AllChampsDeltaWeight;

            foreach (Champion champion in AppCache.Instance.Champions)
            {
                if (champion.Id == -1)
                    continue;
                LeagueChampionMastery? mastery = masteries.FirstOrDefault(m => m.ChampionId == champion.Id);
                if (mastery is not null)
                {
                    double weight = (1d - ((double)mastery.MasteryPoints / (double)maxMastery.MasteryPoints));
                    if (IsEqual(0.001d, weight, 0.01))
                        weight += PlayerMainWeight;
                    championWeights.Add(champion.Id, weight * PlayerMasteryChampionWeightModifier);
                }
                else
                {
                    championsWithoutMastery.Add(champion.Id);
                    championWeights.Add(champion.Id, otherChampsProbability);
                }
            }

            List<LeagueChampion> generatedChampions = new List<LeagueChampion>(amount);

            for (int i = 0; i < amount; i++)
            {
                int championId = championWeights.RandomElementByWeight(cW => cW.Value).Key;

                Champion champ = AppCache.Instance.Champions[championId];
                LeagueChampionMastery mastery = masteries.FirstOrDefault(m => m.ChampionId == champ.Id);
                LeagueChampion champion = new LeagueChampion(champ, mastery, championWeights[champ.Id]); 
                generatedChampions.Add(champion);
                championWeights.Remove(championId);
                championsWithoutMastery.Remove(championId);
            }

            return generatedChampions.ToArray();
        }

        public static bool IsEqual(double a, double b, double delta)
        {
            return Math.Abs(a - b) < delta;
        }
    }
}
