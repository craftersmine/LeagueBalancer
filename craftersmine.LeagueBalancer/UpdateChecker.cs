using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace craftersmine.LeagueBalancer
{
    public class UpdateChecker
    {
        public const string GitHubReleaseUrl =
            "https://api.github.com/repos/craftersmine/LeagueBalancer/releases/latest";

        private static readonly Regex TagNameRegex = new Regex("\"tag_name\":\"(?<tag>.[0-9.a-zA-Z]*)\"");
        private const string LatestReleaseUri = "https://github.com/craftersmine/LeagueBalancer/releases/latest";

        public event EventHandler<NewVersionReleasedEventArgs> NewVersionReleased;

        public UpdateChecker()
        {

        }

        public async void CheckVersion()
        {
            string? infoData = await GetLatestReleaseInfo();
            if (infoData is null)
                return;

            Version newVersion = GetVersion(infoData);
            if (newVersion == new Version(0, 0))
                return;

            if (newVersion > App.CurrentVersion)
                NewVersionReleased?.Invoke(this, new NewVersionReleasedEventArgs(App.CurrentVersion, newVersion, LatestReleaseUri));
        }

        private Version GetVersion(string infoData)
        {
            Match match = TagNameRegex.Match(infoData);
            if (match.Success)
            {
                Version ver = Version.Parse(match.Groups["tag"].Value);
                return ver;
            }

            return new Version(0, 0);
        }

        private async Task<string?> GetLatestReleaseInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("craftersmine.LeagueBalancer", App.CurrentVersion.ToString()));
                try
                {
                    HttpResponseMessage response = await client.GetAsync(GitHubReleaseUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }

    public sealed class NewVersionReleasedEventArgs : EventArgs
    {
        public Version CurrentVersion { get; private set; }
        public Version NewVersion { get; private set; }
        public string ReleaseUrl { get; private set; }

        public NewVersionReleasedEventArgs(Version currentVersion, Version newVersion, string releaseUrl)
        {
            CurrentVersion = currentVersion;
            NewVersion = newVersion;
            ReleaseUrl = releaseUrl;
        }
    }
}
