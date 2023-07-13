using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace craftersmine.LeagueBalancer
{
    /// <summary>
    /// Логика взаимодействия для CrashHandlerWindow.xaml
    /// </summary>
    public partial class CrashHandlerWindow : Window
    {
        public Exception Exception { get; private set; }

        public string Message => Exception.Message;
        public string Type => Exception.GetType().FullName;
        public string StackTrace => GetStackTrace(Exception);

        public CrashHandlerWindow(Exception e)
        {
            Exception = e;
            InitializeComponent();
        }

        private void OpenGithubIssuesClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/craftersmine/LeagueBalancer/issues/new?assignees=&labels=bug&projects=&template=bug_report.md&title=%5BBUG%5D+Bug+report") { UseShellExecute = true });
        }

        private void CloseAppClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string GetStackTrace(Exception ex)
        {
            List<string> lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                foreach (var line in ex.StackTrace.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    lines.Add($"{line}");
            else lines.Add("No Exception Stacktrace recorded!");
            if (ex.InnerException is not null)
                lines.Add(GetStackTrace(ex.InnerException));
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        private void CopyInfoClick(object sender, RoutedEventArgs e)
        {
            string infoFormat = "Exception Type: {0}\r\nException Message: {1}\r\nException StackTrace:\r\n{2}";

            string info = string.Format(infoFormat, Type, Message, StackTrace);
            Clipboard.SetText(info);
        }
    }
}
