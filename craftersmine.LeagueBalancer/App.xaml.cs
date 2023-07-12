using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using craftersmine.League.CommunityDragon;
using craftersmine.Riot.Api.Common;
using craftersmine.Riot.Api.League.Mastery;
using craftersmine.Riot.Api.League.Summoner;
using craftersmine.Riot.Api.League.SummonerLeagues;

namespace craftersmine.LeagueBalancer
{
    public partial class App : Application
    {
        public static RiotApiClientSettings ClientSettings { get; private set; }

        public static LeagueSummonerApiClient SummonerApiClient { get; private set; }
        public static LeagueSummonerLeaguesApiClient SummonerLeaguesApiClient { get; private set; }
        public static LeagueMasteryApiClient MasteryApiClient { get; private set; }

        public static CommunityDragon CommunityDragonClient { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ClientSettings = RiotApiClientSettingsBuilder
                .CreateSettingsBuilder("RGAPI-82748089-fe97-4673-b120-e4543d11f804")
                .UseDefaultDataRegion(RiotRegion.Europe).UseExperimentalLeaguesApi().Build();

            SummonerApiClient = new LeagueSummonerApiClient(ClientSettings);
            SummonerLeaguesApiClient = new LeagueSummonerLeaguesApiClient(ClientSettings);
            MasteryApiClient = new LeagueMasteryApiClient(ClientSettings);

            CommunityDragonClient = new CommunityDragon(VersionAlias.Latest, LeagueLocales.English);

            base.OnStartup(e);
        }
    }
}
