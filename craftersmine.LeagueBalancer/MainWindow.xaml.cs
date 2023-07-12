using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using craftersmine.League.CommunityDragon;
using craftersmine.Riot.Api.Common;
using craftersmine.Riot.Api.Common.Exceptions;
using craftersmine.Riot.Api.League.Summoner;

namespace craftersmine.LeagueBalancer
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _blueTeamLp = 0;
        private int _redTeamLp = 0;

        public ObservableCollection<Summoner> Summoners { get; set; } = new ObservableCollection<Summoner>();
        public ObservableCollection<LeagueRegion> Regions { get; set; } = new ObservableCollection<LeagueRegion>();

        public ObservableCollection<Summoner> BlueTeam { get; set; } = new ObservableCollection<Summoner>();
        public ObservableCollection<Summoner> RedTeam { get; set; } = new ObservableCollection<Summoner>();

        public Dictionary<Summoner, LeagueChampion[]> Champions { get; set; } =
            new Dictionary<Summoner, LeagueChampion[]>();

        public const string ChatMainFormat = "Rolled champions:\r\n{0}";
        public const string ChatSummonerFormat = "  {0}: {1}";

        public int BlueTeamLp
        {
            get => _blueTeamLp;
            set
            {
                _blueTeamLp = value;
                OnPropertyChanged();
            }
        }
        public int RedTeamLp
        {
            get => _redTeamLp;
            set
            {
                _redTeamLp = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            foreach (RiotPlatform region in Enum.GetValues<RiotPlatform>())
            {
                Regions.Add(new LeagueRegion(region));
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            AddSummoner(SummonerNameTextBox.Text);
        }

        private void OnSummonerNameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSummoner(SummonerNameTextBox.Text);
            }
        }

        private async void AddSummoner(string summonerName)
        {
            summonerName = summonerName.Trim();

            IsEnabled = false;
            Cursor = Cursors.Wait;
            if (!string.IsNullOrWhiteSpace(summonerName) && Summoners.FirstOrDefault(s => s.SummonerInfo.Name == summonerName) is null &&
                Summoners.Count < 10)
            {
                try
                {
                    LeagueSummoner sum = await App.SummonerApiClient.GetSummonerByNameAsync((SelectedRegion.SelectedItem as LeagueRegion)!.Region, summonerName);
                    Summoner summoner = new Summoner(sum, (SelectedRegion.SelectedItem as LeagueRegion)!);
                    Summoners.Add(summoner);
                    SummonerNameTextBox.Text = string.Empty;
                }
                catch (Exception e)
                {
                    if (e is RiotApiException rae)
                    {
                        switch (rae.ResponseCode)
                        {
                            case HttpResponseCode.NotFound:
                                MessageBox.Show("Unable to player \"" + summonerName + "\" on " +
                                                (SelectedRegion.SelectedItem as LeagueRegion)!.RegionName + " server! Check if username is correct and selected right server.", "Unable to find player!", MessageBoxButton.OK, MessageBoxImage.Information);
                                break;
                            case HttpResponseCode.Forbidden:
                            case HttpResponseCode.Unauthorized:
                                MessageBox.Show("Unable to access Riot Games API due to issue with Application API key! Contact app developer about this error!", "Expired or broken API key!", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            case HttpResponseCode.RateLimitExceeded:
                                MessageBox.Show("Unable to access Riot Games API due to being rate-limited! Try again after " + App.SummonerApiClient.RetryAfter?.ToString("g"), "Rate-limited!", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                        }
                    }
                }
            }

            if (Summoners.Count == 10)
                AddSummonerButton.IsEnabled = false;
            if (Summoners.Any())
            {
                BalanceButton.IsEnabled = true;
                RandomizeInfoButton.IsEnabled = true;
            }
            Cursor = Cursors.Arrow;
            IsEnabled = true;
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            if (SummonersListBox.SelectedItem is not null)
            {
                Summoner selectedSummoner = (SummonersListBox.SelectedItem as Summoner)!;
                Summoners.Remove(selectedSummoner);
                if (Summoners.Count < 10)
                    AddSummonerButton.IsEnabled = true;
                if (Summoners.Any())
                {
                    BalanceButton.IsEnabled = true;
                    RandomizeInfoButton.IsEnabled = true;
                    RerollChampionButton.IsEnabled = true;
                }
                else
                {
                    BalanceButton.IsEnabled = false;
                    RandomizeInfoButton.IsEnabled = false;
                    RemoveSummonerButton.IsEnabled = false;
                    RerollChampionButton.IsEnabled = false;
                }
            }
        }

        private void OnSummonersListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SummonersListBox.SelectedItem is not null)
                RemoveSummonerButton.IsEnabled = true;

            if (Champions.Any())
            // TODO: implement list of champions on the right
            {
                Summoner? summoner = SummonersListBox.SelectedItem as Summoner;
                if (summoner is null)
                {
                    SelectPlayerTip.Visibility = Visibility.Visible;
                    SelectedSummonerChampions.Visibility = Visibility.Hidden;
                    return;
                }

                if (Champions.ContainsKey(summoner))
                {
                    RerollChampionButton.IsEnabled = true;
                    SelectPlayerTip.Visibility = Visibility.Hidden;
                    RandomizedInfoSpinner.Visibility = Visibility.Hidden;
                    SelectedSummonerChampions.ItemsSource = Champions[summoner];
                    SelectedSummonerChampions.Visibility = Visibility.Visible;
                }
                else
                {
                    SelectedSummonerChampions.Visibility = Visibility.Hidden;
                    SelectPlayerTip.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnBalancedSummonersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            e.Handled = true;
        }

        private void OnBalanceClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            Cursor = Cursors.Wait;
            TeamSpinner.Visibility = Visibility.Visible;
            BlueTeam.Clear();
            RedTeam.Clear();
            Dictionary<LeagueTeamType, LeagueTeam> balancedTeams = Balancer.BalanceTeams(Summoners.ToArray());
            foreach (var team in balancedTeams)
            {
                switch (team.Key)
                {
                    case LeagueTeamType.Blue:
                        BlueTeamLp = team.Value.LeaguePointsTotal;
                        foreach (Summoner summoner in team.Value.Summoners)
                        {
                            BlueTeam.Add(summoner);
                        }
                        break;
                    case LeagueTeamType.Red:
                        RedTeamLp = team.Value.LeaguePointsTotal;
                        foreach (Summoner summoner in team.Value.Summoners)
                        {
                            RedTeam.Add(summoner);
                        }
                        break;
                }
            }

            IsEnabled = true;
            Cursor = Cursors.Arrow;
            TeamSpinner.Visibility = Visibility.Hidden;
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

        private async void OnRandomizeInfoClick(object sender, RoutedEventArgs e)
        {
            Champions.Clear();
            CopyChampionsToClipboard.IsEnabled = false;
            SelectedSummonerChampions.Visibility = Visibility.Hidden;
            SelectedSummonerChampions.ItemsSource = null;
            SummonersListBox.SelectedItem = null;
            RandomizedInfoSpinner.Visibility = Visibility.Visible;
            SelectPlayerTip.Visibility = Visibility.Hidden;
            IsEnabled = false;

            if (!Summoners.Any())
                return;

            foreach (Summoner summoner in Summoners)
            {
                LeagueChampion[] champions = await Balancer.GetChampionList(summoner, (int)AvailablePoolSlider.Value, 100);
                Champions.Add(summoner, champions);
            }

            RandomizedInfoSpinner.Visibility = Visibility.Hidden;
            CopyChampionsToClipboard.IsEnabled = true;
            SelectPlayerTip.Visibility = Visibility.Visible;
            IsEnabled = true;
        }

        private async void OnRerollChampionInfoClick(object sender, RoutedEventArgs e)
        {
            Summoner? summoner = SummonersListBox.SelectedItem as Summoner;
            if (summoner is not null)
            {
                try
                {
                    CopyChampionsToClipboard.IsEnabled = false;
                    RandomizedInfoSpinner.Visibility = Visibility.Visible;
                    SelectPlayerTip.Visibility = Visibility.Hidden;
                    SelectedSummonerChampions.Visibility = Visibility.Hidden;
                    IsEnabled = false;
                    LeagueChampion[] champions = await Balancer.GetChampionList(summoner, (int)AvailablePoolSlider.Value, 100);
                    Champions[summoner] = champions;
                    SelectedSummonerChampions.ItemsSource = Champions[summoner];
                    SummonersListBox.SelectedItem = summoner;
                    RandomizedInfoSpinner.Visibility = Visibility.Hidden;
                    SelectedSummonerChampions.Visibility = Visibility.Visible;
                    CopyChampionsToClipboard.IsEnabled = true;
                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    if (ex is RiotApiException rae)
                    {
                        switch (rae.ResponseCode)
                        {
                            case HttpResponseCode.Forbidden:
                            case HttpResponseCode.Unauthorized:
                                MessageBox.Show("Unable to access Riot Games API due to issue with Application API key! Contact app developer about this error!", "Expired or broken API key!", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            case HttpResponseCode.RateLimitExceeded:
                                MessageBox.Show("Unable to access Riot Games API due to being rate-limited! Try again after " + App.SummonerApiClient.RetryAfter?.ToString("g"), "Rate-limited!", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                        }
                    }
                }
            }
        }

        private void ImportSummonersFromClipBoardClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            Cursor = Cursors.Wait;
            string[] summoners = InputSanitizer.SanitizeInputFromChat(Clipboard.GetText());
            foreach (string s in summoners)
            {
                AddSummoner(s);
            }

            Cursor = Cursors.Arrow;
            IsEnabled = true;
        }

        private void CopyChampionsToClipboardClick(object sender, RoutedEventArgs e)
        {
            List<string> summoners = new List<string>();
            foreach (var champion in Champions)
            {
                string[] champNames = new string[champion.Value.Length];
                for (int i = 0; i < champion.Value.Length; i++)
                    champNames[i] = champion.Value[i].Champion.Name;
                summoners.Add(string.Format(ChatSummonerFormat, champion.Key.SummonerInfo.Name, string.Join(", ", champNames)));
            }

            string message = string.Format(ChatMainFormat, string.Join(Environment.NewLine, summoners));
            Clipboard.SetText(message);
        }
    }
}
