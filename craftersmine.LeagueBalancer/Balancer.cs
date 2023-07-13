using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using craftersmine.League.CommunityDragon;
using craftersmine.Riot.Api.Common;
using craftersmine.Riot.Api.League.Mastery;

namespace craftersmine.LeagueBalancer
{
    public class Balancer
    {
        private const double AllChampsDeltaWeight = 0.002d;
        private const double PlayerMainWeight = 0.3d;
        private const double PlayerMasteryChampionWeightModifier = 0.95d;

        public static Dictionary<LeagueTeamType, LeagueTeam> BalanceTeams(Summoner[] summoners)
        {
            List<Summoner> blueTeam = new List<Summoner>();
            List<Summoner> redTeam = new List<Summoner>();

            int averageLp = (int)summoners.Average(s => s.LeaguePointsAmount);
            int blueTeamLp = 0;
            int redTeamLp = 0;

            List<Summoner> orderedSummoners = summoners.OrderBy(s => s.LeaguePointsAmount).ToList();

            while (orderedSummoners.Any())
            {
                Summoner summoner = orderedSummoners.MaxBy(s => s.LeaguePointsAmount)!;

                if (blueTeamLp < redTeamLp)
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


            Dictionary<LeagueTeamType, LeagueTeam> teams = new Dictionary<LeagueTeamType, LeagueTeam>
            {
                { LeagueTeamType.Blue, new LeagueTeam(blueTeam.ToArray()) },
                { LeagueTeamType.Red, new LeagueTeam(redTeam.ToArray()) }
            };
            return teams;
        }

        public static async Task<LeagueChampion[]> GetChampionList(Summoner summoner, int amount, int gamesToGet)
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
                    double weight = 1d - ((double)mastery.MasteryPoints / (double)maxMastery.MasteryPoints) * PlayerMasteryChampionWeightModifier;
                    double weight = (1d - ((double)mastery.MasteryPoints / (double)maxMastery.MasteryPoints)) * PlayerMasteryChampionWeightModifier;
                    if (IsEqual(0d, weight, 0.00001))
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
                LeagueChampion champion = new LeagueChampion(champ, mastery); 
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
