using System;
using System.Windows.Forms;

namespace BrowserApp
{
    public partial class Form1 : Form
    {
        public Form1(string url) : this()
        {
            webBrowser1.Url = new Uri(url);
        }

        public Form1()
        {
            InitializeComponent();
            webBrowser1.DocumentCompleted += (s,
                                              e) =>
            { 
                Text = webBrowser1.Document.Title;
            };
        }

        SHDocVw.WebBrowser nativeBrowser;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            nativeBrowser = (SHDocVw.WebBrowser)webBrowser1.ActiveXInstance;
            nativeBrowser.NewWindow2 += nativeBrowser_NewWindow2;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (nativeBrowser != null)
            {
                nativeBrowser.NewWindow2 -= nativeBrowser_NewWindow2;
            }

            base.OnFormClosing(e);
        }

        void nativeBrowser_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            var popup = new Form1();
            popup.Show(this);
            ppDisp = popup.webBrowser1.ActiveXInstance;
        }
    }
}
