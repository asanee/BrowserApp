using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserApp
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        unsafe static void Main()
        {
            int option = (int)3 /* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
            int* optionPtr = &option;

            bool success = InternetSetOption(0, 81 /*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
            if (!success)
            {
                MessageBox.Show("Something went wrong !>?");

                return;
            }

            SetIE8KeyforWebBrowserControl(Process.GetCurrentProcess().ProcessName + ".exe");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1("http://www.idateasia.com/lady"));
            //Application.Run(new Form1("http://localhost:9090"));
        }

        [DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private static void SetIE8KeyforWebBrowserControl(string appName)
        {
            RegistryKey regkey = null;
            try
            {

                //For 64 bit Machine 
                if (Environment.Is64BitOperatingSystem)
                {
                    regkey =
                        Registry.LocalMachine.OpenSubKey(
                            @"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION",
                            true);
                }
                else //For 32 bit Machine 
                {
                    regkey =
                        Registry.LocalMachine.OpenSubKey(
                            @"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION",
                            true);
                }

                //If the path is not correct or 
                //If user't have priviledges to access registry 
                if (regkey == null)
                {
                    MessageBox.Show("Application Settings Failed - Address Not found");
                    return;
                }

                string FindAppkey = Convert.ToString(regkey.GetValue(appName));

                //Check if key is already present 
                if (FindAppkey == "8000")
                {
                    MessageBox.Show("Required Application Settings Present");
                    regkey.Close();
                    return;
                }

                //If key is not present add the key , Kev value 8000-Decimal 
                if (string.IsNullOrEmpty(FindAppkey))
                    regkey.SetValue(appName, unchecked((int)0x1F40), RegistryValueKind.DWord);

                //check for the key after adding 
                FindAppkey = Convert.ToString(regkey.GetValue(appName));

                if (FindAppkey == "8000")
                {
                    MessageBox.Show("Application Settings Applied Successfully");
                }
                else
                {
                    MessageBox.Show("Application Settings Failed, Ref: " + FindAppkey);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application Settings Failed");
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Close the Registry 
                if (regkey != null)
                {
                    regkey.Close();
                }
            }
        }
    }
}
