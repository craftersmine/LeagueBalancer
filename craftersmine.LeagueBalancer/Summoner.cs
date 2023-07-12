using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using craftersmine.Riot.Api.Common;
using craftersmine.Riot.Api.League.Summoner;
using craftersmine.Riot.Api.League.SummonerLeagues;

namespace craftersmine.LeagueBalancer
{
    public class Summoner : INotifyPropertyChanged
    {
        private Uri _iconUri;
        private ImageSource _iconSource;
        private LeagueSummoner _leagueSummoner;
        private SummonerLeague _summonerLeague;
        private string _summonerLeagueString;
        private int _lpAmount;

        public Uri IconUri
        {
            get => _iconUri;
            private set
            {
                _iconUri = value;
                OnPropertyChanged(nameof(IconUri));
            }
        }

        public ImageSource Icon
        {
            get => _iconSource;
            set
            {
                _iconSource = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public LeagueSummoner SummonerInfo
        {
            get => _leagueSummoner;
            set
            {
                _leagueSummoner = value;
                OnPropertyChanged(nameof(SummonerInfo));
            }
        }

        public SummonerLeague? SummonerLeague
        {
            get => _summonerLeague;
            set
            {
                _summonerLeague = value;
                OnPropertyChanged(nameof(SummonerLeague));
            }
        }

        public LeagueRegion Region { get; set; }

        public string SummonerLeagueString
        {
            get => _summonerLeagueString;
            set
            {
                _summonerLeagueString = value;
                OnPropertyChanged(nameof(SummonerLeagueString));
            }
        }

        public int LeaguePointsAmount
        {
            get => _lpAmount;
            set
            {
                _lpAmount = value;
                OnPropertyChanged(nameof(LeaguePointsAmount));
            }
        }

        public Summoner(LeagueSummoner summoner, LeagueRegion region)
        {
            SummonerInfo = summoner;
            Region = region;
            Initialize();
        }

        public async void Initialize()
        {
            SummonerLeague[] leagues =
                await App.SummonerLeaguesApiClient.GetLeagueEntriesForSummonerByIdAsync(Region.Region, SummonerInfo.Id);

            foreach (var league in leagues)
            {
                if (SummonerLeague is null)
                    SummonerLeague = league;
                if ((int)league.Tier > (int)SummonerLeague.Tier)
                    SummonerLeague = league;
            }

            SummonerLeagueString = LeagueRankedTier.Unranked.ToString();
            if (SummonerLeague is not null)
            {
                LeaguePointsAmount = CalculateLpValue(SummonerLeague.Tier, SummonerLeague.DivisionRank,
                    SummonerLeague.LeaguePoints);
                switch (SummonerLeague.Tier)
                {
                    case LeagueRankedTier.Iron:
                    case LeagueRankedTier.Bronze:
                    case LeagueRankedTier.Silver:
                    case LeagueRankedTier.Gold:
                    case LeagueRankedTier.Platinum:
                    case LeagueRankedTier.Diamond:
                        SummonerLeagueString = SummonerLeague.Tier.ToString() + " " + SummonerLeague.DivisionRank.ToString() + " (" + LeaguePointsAmount + " LP)";
                        break;
                    case LeagueRankedTier.Master:
                    case LeagueRankedTier.Grandmaster:
                    case LeagueRankedTier.Challenger:
                        SummonerLeagueString = SummonerLeague.Tier.ToString() + " (" + LeaguePointsAmount + " LP)";
                        break;
                    default:
                        SummonerLeagueString = LeagueRankedTier.Unranked.ToString();
                        LeaguePointsAmount = 0;
                        break;
                }
            }

            if (AppCache.Instance.Icons is null || !AppCache.Instance.Icons.Any())
                AppCache.Instance.Icons = await App.CommunityDragonClient.GetSummonerIconsAsync();

            IconUri = new Uri(AppCache.Instance.Icons[SummonerInfo.ProfileIconId].GetAssetUri());
            Icon = new BitmapImage(IconUri);
        }

        public static int CalculateLpValue(LeagueRankedTier tier, LeagueDivisionRank division, int currentLp)
        {
            if (tier == LeagueRankedTier.Unranked || tier == LeagueRankedTier.Unknown)
                return 0;

            int tierLpValue = 0;
            switch (tier)
            {
                case LeagueRankedTier.Iron:
                case LeagueRankedTier.Bronze:
                case LeagueRankedTier.Silver:
                case LeagueRankedTier.Gold:
                case LeagueRankedTier.Platinum:
                case LeagueRankedTier.Diamond:
                case LeagueRankedTier.Master:
                    tierLpValue = ((int)tier - 1) * 400;
                    break;
                case LeagueRankedTier.Grandmaster:
                    tierLpValue = 2400 + 450;
                    break;
                case LeagueRankedTier.Challenger:
                    tierLpValue = 2400 + 450 + 700;
                    break;
            }

            if (tier != LeagueRankedTier.Master && tier != LeagueRankedTier.Grandmaster && tier != LeagueRankedTier.Challenger)
                switch (division)
                {
                    case LeagueDivisionRank.I:
                        tierLpValue += 300;
                        break;
                    case LeagueDivisionRank.II:
                        tierLpValue += 200;
                        break;
                    case LeagueDivisionRank.III:
                        tierLpValue += 100;
                        break;
                    case LeagueDivisionRank.IV:
                        tierLpValue += 0;
                        break;
                }

            tierLpValue += currentLp;
            return tierLpValue;
        }

        public override string ToString()
        {
            return SummonerInfo.Name;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
