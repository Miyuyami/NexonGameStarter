using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace GameStarter
{
    public enum StartMode
    {
        Launch = 0,
        Patch,
    }

    public partial class BrowserForm : MyForm
    {
        private const string UrlFormat = "ngm://launch/ -locale:KR -mode:{3} -game:{0}:0 -token:'{1}' -a2sk:'{2}'";
        private const string LoginUrl = "https://clogin.nexon.com/common/clogin.aspx";

        private Dictionary<StartMode, string> StartModes = new Dictionary<StartMode, string>()
        {
            { StartMode.Launch, "launch" },
            { StartMode.Patch, "restore" },
        };

        public string SelectedMode => this.StartModes[(StartMode)this.ModeToolStripComboBox.SelectedItem];

        public BrowserForm()
        {
            this.InitializeComponent();

            this.ModeToolStripComboBox.ComboBox.DataSource = this.StartModes.Keys.ToList();
        }

        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (sender is WebBrowser webBrowser)
            {
                Dictionary<string, string> cookies = webBrowser.GetCookies();

                if (!cookies.TryGetValue("NPP", out string nppString))
                {
                    return;
                }

                if (!cookies.TryGetValue("A2SK", out string a2skString))
                {
                    return;
                }

                string launchUri = String.Format(UrlFormat, Arguments.GameId, nppString, a2skString, this.SelectedMode);
                string escapedUri = Uri.EscapeUriString(launchUri);

                e.Cancel = true;
                webBrowser.AllowNavigation = false;
                webBrowser.Visible = false;

                this.EndBrowsing(escapedUri);
            }
        }

        private void EndBrowsing(string url)
        {
            this.Visible = false;

            Process.Start(url);

            Application.Exit();
        }

        private void LoginPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WebBrowser.Navigate(LoginUrl);
        }
    }
}
