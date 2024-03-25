using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using craftersmine.League.CommunityDragon;
using craftersmine.Ui.League.Controls;

namespace craftersmine.LeagueBalancer
{
    public partial class ApplicationSettingsWindow : LeagueWindow
    {
        public static ObservableCollection<CultureInfo> Cultures { get; private set; }

        public ApplicationSettingsWindow()
        {
            Cultures = new ObservableCollection<CultureInfo>();
            Cultures.Add(new CultureInfo("en-US"));
            foreach (CultureInfo info in App.GetAvailableCultures())
                Cultures.Add(info);

            InitializeComponent();

            SelectedLangComboBox.SelectedItem = SelectedLangComboBox.Items.Cast<CultureInfo>().FirstOrDefault(c => c.Name == Settings.Default.Language);
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            string lang = ((CultureInfo)SelectedLangComboBox.SelectedValue).Name;
            if (Settings.Default.Language == lang)
                MessageBox.Show("It is required to restart application in order to apply language!",
                    "Restart required!", MessageBoxButton.OK, MessageBoxImage.Information);
            Settings.Default.Language = lang;
            Settings.Default.Save();

            this.Close();
        }
    }
}
