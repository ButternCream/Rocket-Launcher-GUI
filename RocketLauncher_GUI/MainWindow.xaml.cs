using PcapDotNet.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace RocketLauncher_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* Global Vars */
        Thread procWatcher;
        public delegate void updateLabel(string text);
        Thread updateThread;
        volatile static bool abort = false;

        // Import c++ inject function
        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            RLMenuBarInit();
            //Start thread to check if RL is running / if dll is injected
            updateThread = new Thread(new ThreadStart(CheckForRL));
            updateThread.Name = "Update Thread";
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        /*
         * If user path has been found enable rocket league menu option 
         */
        private void RLMenuBarInit()
        {
            if (Properties.Settings.Default.Path == String.Empty)
            {
                Rocket_League.IsEnabled = false;
            }
            else
            {
                Rocket_League.IsEnabled = true;
            }
        }

        /*
         * Start auto injector if enabled and process watcher if path is empty
         */
        private void LoadSettings()
        {
            btnAutoLoadMods.IsChecked = Properties.Settings.Default.AutoLoadMods;
            if (btnAutoLoadMods.IsChecked && !IsModuleLoaded("RLModding.dll"))
            {
                Console.WriteLine("Started AutoLoad thread");
                abort = false;
                Thread injectWatcher = new Thread(() => AutoLoadMods());
                injectWatcher.IsBackground = true;
                injectWatcher.Start();
            }
            if (Properties.Settings.Default.Path == String.Empty)
            {
                procWatcher = new Thread(new ThreadStart(GetProcInfo));
                procWatcher.IsBackground = true;
                procWatcher.Start();
            }
            
        }

        /*
         * Check if RL is running and if not, update the injector label
         */
        private void CheckForRL()
        {
            while (true)
            {
                Process[] procs = Process.GetProcessesByName("RocketLeague");
                if (procs.Length < 1 || !IsModuleLoaded("RLModding.dll"))
                {
                    // Change inject label using delegate callback
                    updateLabel delUpadte = new updateLabel(UpdateUI);
                    this.injectLbl.Dispatcher.BeginInvoke(delUpadte, "Not Injected");
                    
                }
                else { Thread.Sleep(5000); }
            }
        }

        /*
         * Update injector label
         */
        private void UpdateUI(string text)
        {
            this.injectLbl.Content = text;
        }

        /*
         * Get users RL path if it doesn't exists
         */
        private static void GetProcInfo()
        {
            if (Properties.Settings.Default.Path != String.Empty)
            {
                return;
            }
        
            while (Properties.Settings.Default.Path == String.Empty)
            {
                Process[] procs = Process.GetProcessesByName("RocketLeague");
                if (procs.Length > 0)
                {
                    try
                    {
                        string path = procs[0].MainModule.FileName.Replace("\\", "\\\\");
                        if (path.ToLower().Contains("rocketleague"))
                        { 
                            Properties.Settings.Default.Path = path.Remove(path.Length - 16);
                            Properties.Settings.Default.Save();
                            return;

                        }
                    } catch (Exception e)
                    {
                        return;
                    }
                } else { Thread.Sleep(2000); }
            }

        }

        /*
         * Auto injector function
         */
        private static void AutoLoadMods()
        {
            if (Properties.Settings.Default.AutoLoadMods)
            {
                bool injected = false;
                while (!injected && !abort)
                {
                    Process[] procs = Process.GetProcessesByName("RocketLeague");
                    if (procs.Length > 0)
                    {
                        
                        Thread.Sleep(15000);
                        if (!IsModuleLoaded("RLModding.dll"))
                        {
                            Console.WriteLine("Injecting");
                            System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                            sound.Play();
                            injected = Inject();
                        }
                    }
                    else { Thread.Sleep(1000); }
                }
                Console.WriteLine("Exiting Thread");
            }
        }

        /* Donate */
         private void btnDonate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XHLHQGAQK2XZG");
        }

        
        /* Load Mods button click */
        private void btnLoadMods_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModuleLoaded("RLModding.dll") && Inject())
            {
                System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                sound.Play();
                injectLbl.Content = "Injected";
            }
            else if (IsModuleLoaded("RLModding.dll"))
            {
                injectLbl.Content = "Injected";
            }
              
        }

        /* Check if dll is injected */
        private static bool IsModuleLoaded(String ModuleName)
        {
            var q = from p in Process.GetProcessesByName("RocketLeague")
                    from m in p.Modules.OfType<ProcessModule>()
                    select m;
            return q.Any(pm => pm.ModuleName.Contains(ModuleName));
        }

        /*
         * Start thread for auto injector if the user changed their 'Auto Load Mods' setting
         */
        private void btnAutoLoadMods_Clicked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoLoadMods = btnAutoLoadMods.IsChecked;
            Properties.Settings.Default.Save();
            if (btnAutoLoadMods.IsChecked && !IsModuleLoaded("RLModding.dll"))
            {
                Console.WriteLine("Started AutoLoad thread");
                abort = false;
                Thread injectWatcher = new Thread(() => AutoLoadMods());
                injectWatcher.IsBackground = true;
                injectWatcher.Start();
            }
            else if (!btnAutoLoadMods.IsChecked)
            {
                Console.WriteLine("Aborting AutoLoad thread");
                abort = true;
            }
        }

        /* Start RL */
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            String RL = Properties.Settings.Default.Path + "RocketLeague.exe";
            Process.Start(RL);
        }

        /* Kill Rocket League Process */
        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("RocketLeague");
            if (procs.Length > 0)
            {
                procs[0].Kill();
            }
        }

        /* Intercept wireless devices for direct IP to send requests to specified ip */
        private void InterceptDev(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> serverList = new List<string>() { IP.Text };
                Support_Files.Simulator.Intercept(serverList);
            }
            catch (Exception)
            {
                Notice notice = new Notice();
                notice.Show();
            }
            
        }

        /* Download WinPcap and run the installer */
        Uri uri = new Uri("https://www.winpcap.org/install/bin/WinPcap_4_1_3.exe");
        string path = Directory.GetCurrentDirectory() + "/WinPcap.exe";
        private void WinPcap_Download_Handler(object sender, RoutedEventArgs e)
        {
            
            try
            {
                download_bar.Visibility = Visibility.Visible;
                if (File.Exists(path))
                {
                    File.Delete(path);
                } 
                WebClient wc = new WebClient();
                wc.DownloadFileAsync(uri, path);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            }
            catch (Exception)
            {
                return;
            }

        }

        /* Update the progress bar */
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            download_bar.Value = e.ProgressPercentage;
            if (download_bar.Value == download_bar.Maximum)
            {
                download_bar.Value = 0;
            }
        }

        /* Start WinPcap installer after download completes */
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Process.Start(path);
                download_bar.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Error running WinPcap installer, check connection");
            }
        }
    }
}
