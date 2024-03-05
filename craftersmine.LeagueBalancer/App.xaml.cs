using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using craftersmine.League.CommunityDragon;
using craftersmine.Riot.Api.Account;
using craftersmine.Riot.Api.Common;
using craftersmine.Riot.Api.League.Mastery;
using craftersmine.Riot.Api.League.Summoner;
using craftersmine.Riot.Api.League.SummonerLeagues;

namespace craftersmine.LeagueBalancer
{
    public partial class App : Application
    {
        public static readonly Version CurrentVersionPredefined = new Version(1, 1, 5);

        public static RiotApiClientSettings ClientSettings { get; private set; }

        public static LeagueSummonerApiClient SummonerApiClient { get; private set; }
        public static LeagueSummonerLeaguesApiClient SummonerLeaguesApiClient { get; private set; }
        public static LeagueMasteryApiClient MasteryApiClient { get; private set; }
        public static RiotAccountApiClient RiotAccountApiClient { get; private set; }

        public static CommunityDragon CommunityDragonClient { get; private set; }

        public static Version CurrentVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-ru");
            CultureInfo.DefaultThreadCurrentUICulture  = new CultureInfo("ru-ru");

            ClientSettings = RiotApiClientSettingsBuilder
                .CreateSettingsBuilder(KeyManager.RetrieveKey())
                .UseDefaultDataRegion(RiotRegion.Europe).UseExperimentalLeaguesApi().Build();

            SummonerApiClient = new LeagueSummonerApiClient(ClientSettings);
            SummonerLeaguesApiClient = new LeagueSummonerLeaguesApiClient(ClientSettings);
            MasteryApiClient = new LeagueMasteryApiClient(ClientSettings);
            RiotAccountApiClient = new RiotAccountApiClient(ClientSettings);

            CommunityDragonClient = new CommunityDragon(VersionAlias.Latest, GetValidLeagueCultureInfo(CultureInfo.DefaultThreadCurrentUICulture));

            base.OnStartup(e);
        }

        public IEnumerable<CultureInfo> GetAvailableCultures()
        {
            var programLocation = Process.GetCurrentProcess().MainModule.FileName;
            var resourceFileName = Path.GetFileNameWithoutExtension(programLocation) + ".resources.dll";
            var rootDir = new DirectoryInfo(Path.GetDirectoryName(programLocation));
            return from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
                join d in rootDir.EnumerateDirectories() on c.IetfLanguageTag equals d.Name
                where d.EnumerateFiles(resourceFileName).Any()
                select c;
        }

        public CultureInfo GetValidLeagueCultureInfo(CultureInfo cultureInfo)
        {
            if (LeagueLocales.English.Name == cultureInfo.Name)
                return LeagueLocales.English;
            if (LeagueLocales.Chinese.Name == cultureInfo.Name)
                return LeagueLocales.Chinese;
            if (LeagueLocales.ChineseMY.Name == cultureInfo.Name)
                return LeagueLocales.ChineseMY;
            if (LeagueLocales.ChineseTW.Name == cultureInfo.Name)
                return LeagueLocales.ChineseTW;
            if (LeagueLocales.Czech.Name == cultureInfo.Name)
                return LeagueLocales.Czech;
            if (LeagueLocales.EnglishAU.Name == cultureInfo.Name)
                return LeagueLocales.EnglishAU;
            if (LeagueLocales.EnglishGB.Name == cultureInfo.Name)
                return LeagueLocales.EnglishGB;
            if (LeagueLocales.EnglishPH.Name == cultureInfo.Name)
                return LeagueLocales.EnglishPH;
            if (LeagueLocales.EnglishSG.Name == cultureInfo.Name)
                return LeagueLocales.EnglishSG;
            if (LeagueLocales.French.Name == cultureInfo.Name)
                return LeagueLocales.French;
            if (LeagueLocales.German.Name == cultureInfo.Name)
                return LeagueLocales.German;
            if (LeagueLocales.Greek.Name == cultureInfo.Name)
                return LeagueLocales.Greek;
            if (LeagueLocales.Hungarian.Name == cultureInfo.Name)
                return LeagueLocales.Hungarian;
            if (LeagueLocales.Italian.Name == cultureInfo.Name)
                return LeagueLocales.Italian;
            if (LeagueLocales.Japanese.Name == cultureInfo.Name)
                return LeagueLocales.Japanese;
            if (LeagueLocales.Korean.Name == cultureInfo.Name)
                return LeagueLocales.Korean;
            if (LeagueLocales.Polish.Name == cultureInfo.Name)
                return LeagueLocales.Polish;
            if (LeagueLocales.Portuguese.Name == cultureInfo.Name)
                return LeagueLocales.Portuguese;
            if (LeagueLocales.Romanian.Name == cultureInfo.Name)
                return LeagueLocales.Romanian;
            if (LeagueLocales.Russian.Name == cultureInfo.Name)
                return LeagueLocales.Russian;
            if (LeagueLocales.SpahishAR.Name == cultureInfo.Name)
                return LeagueLocales.SpahishAR;
            if (LeagueLocales.SpanishMX.Name == cultureInfo.Name)
                return LeagueLocales.SpanishMX;
            if (LeagueLocales.Spanish.Name == cultureInfo.Name)
                return LeagueLocales.Spanish;
            if (LeagueLocales.Thai.Name == cultureInfo.Name)
                return LeagueLocales.Thai;
            if (LeagueLocales.Turkish.Name == cultureInfo.Name)
                return LeagueLocales.Turkish;
            if (LeagueLocales.Vietnamese.Name == cultureInfo.Name)
                return LeagueLocales.Vietnamese;

            return LeagueLocales.English;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        private void HandleException(Exception e)
        {
            CrashHandlerWindow crashHandler = new CrashHandlerWindow(e);
            crashHandler.ShowDialog();
            Environment.Exit(e.HResult);
        }
    }
}
