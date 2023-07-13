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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ClientSettings = RiotApiClientSettingsBuilder
                .CreateSettingsBuilder(KeyManager.RetrieveKey())
                .UseDefaultDataRegion(RiotRegion.Europe).UseExperimentalLeaguesApi().Build();

            SummonerApiClient = new LeagueSummonerApiClient(ClientSettings);
            SummonerLeaguesApiClient = new LeagueSummonerLeaguesApiClient(ClientSettings);
            MasteryApiClient = new LeagueMasteryApiClient(ClientSettings);

            CommunityDragonClient = new CommunityDragon(VersionAlias.Latest, LeagueLocales.English);

            base.OnStartup(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CrashHandlerWindow crashHandler = new CrashHandlerWindow((Exception)e.ExceptionObject);
            crashHandler.ShowDialog();
            Environment.Exit(((Exception)e.ExceptionObject).HResult);
        }
    }
}
