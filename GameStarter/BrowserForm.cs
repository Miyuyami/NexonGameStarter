using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameStarter.Properties;
using Microsoft.Web.WebView2.Core;

namespace GameStarter
{
    public partial class BrowserForm : Form
    {
        private const string UrlFormat = "ngm://launch/ -dll:platform.nexon.com/NGM/Bin/NGMDll.dll:1 -locale:{4} -mode:{3} -game:{0}:0 -token:'{1}' -passarg:'null' -a2sk:'{2}' -architectureplatform:'{5}'";
        private const string WebView2DownloadLink = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

        private readonly Dictionary<StartMode, string> StartModes = new Dictionary<StartMode, string>()
        {
            { StartMode.Launch, "launch" },
            { StartMode.Patch, "restore" },
        };

        private readonly Dictionary<Architecture, string> StartArchitectures = new Dictionary<Architecture, string>()
        {
            { Architecture.Auto, "auto" },
            { Architecture.x32, "x86" },
            { Architecture.x64, "x64" },
        };

        public BrowserForm()
        {
            this.InitializeComponent();

            this.ModeToolStripComboBox.ComboBox.DataSource = this.StartModes.Keys.ToList();
            this.ModeToolStripComboBox.SelectedItem = Settings.Default.StartMode;

            this.ArchitectureToolStripComboBox.ComboBox.DataSource = this.StartArchitectures.Keys.ToList();
            this.ArchitectureToolStripComboBox.SelectedItem = Settings.Default.Architecture;
        }

        private async void BrowserForm_Load(object sender, EventArgs e)
        {
            try
            {
                await this.WebView.EnsureCoreWebView2Async();
            }
            catch
            {
                ShowDownloadMessageBox();
            }
        }

        private static void ShowDownloadMessageBox()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Press OK to Download the WebView2 runtime using the Evergreen Bootstrapper. It will be downloaded using your default browser.");
            sb.AppendLine("Install it then restart the application.");
            sb.AppendLine();
            sb.AppendLine(WebView2DownloadLink);

            DialogResult dr = MessageBox.Show(sb.ToString(), "WebView2 Runtime Initialization Failed", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr == DialogResult.OK)
            {
                StartUrlProcess(WebView2DownloadLink);
            }
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                throw e.InitializationException;
            }

            this.WebView.CoreWebView2.Navigate(Arguments.LoginRedirectUrl);
        }

        private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine($"Navigated: {this.WebView.CoreWebView2.Source}");

            List<CoreWebView2Cookie> cookieList = await this.WebView.CoreWebView2.CookieManager.GetCookiesAsync(null);
            Dictionary<string, CoreWebView2Cookie> cookies = cookieList.ToDictionaryDupes(c => c.Name, c => c);

            if (!cookies.TryGetValue("NPP", out CoreWebView2Cookie nppCookie))
            {
                return;
            }
            Debug.WriteLine("Found NPP");

            if (!cookies.TryGetValue("A2SK", out CoreWebView2Cookie a2skCookie))
            {
                return;
            }
            Debug.WriteLine("Found A2SK");

            this.WebView.Visible = false;

            this.FoundCookies(nppCookie, a2skCookie);
        }

        private void FoundCookies(CoreWebView2Cookie nppCookie, CoreWebView2Cookie a2skCookie)
        {
            string nppString = nppCookie.Value;
            string a2skString = a2skCookie.Value;

            string mode = this.StartModes[(StartMode)this.ModeToolStripComboBox.SelectedItem];
            string architecture = this.StartArchitectures[(Architecture)this.ArchitectureToolStripComboBox.SelectedItem];
            string locale = Arguments.StartLocale;
            string launchUri = String.Format(UrlFormat, Arguments.GameId, nppString, a2skString, mode, locale, architecture);
            string escapedUri = Uri.EscapeUriString(launchUri);

            this.CloseApp(escapedUri);
        }

        private void CloseApp(string url)
        {
            this.Visible = false;

            try
            {
                StartUrlProcess(url);
            }
            finally
            {
                Application.Exit();
            }
        }

        private static void StartUrlProcess(string url)
        {
            var psi = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
            };

            Process.Start(psi);
        }

        private void LoginPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WebView.CoreWebView2.Navigate(Arguments.LoginRedirectUrl);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Settings.Default.IsMaximised)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(Settings.Default.DesktopBounds)))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.DesktopBounds = Settings.Default.DesktopBounds;
                this.WindowState = FormWindowState.Normal;
            }

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Settings.Default.IsMaximised = this.WindowState == FormWindowState.Maximized;
            if (this.WindowState == FormWindowState.Minimized)
            {
                Settings.Default.DesktopBounds = this.RestoreBounds;
            }
            else
            {
                Settings.Default.DesktopBounds = this.DesktopBounds;
            }

            Settings.Default.StartMode = (StartMode)this.ModeToolStripComboBox.SelectedItem;
            Settings.Default.Architecture = (Architecture)this.ArchitectureToolStripComboBox.SelectedItem;

            Settings.Default.Save();

            base.OnFormClosing(e);
        }
    }
}
