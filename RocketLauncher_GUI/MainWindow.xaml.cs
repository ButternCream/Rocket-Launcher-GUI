using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

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
        Thread injectWatcher;
        volatile static bool abort = false;
        volatile static bool auto_inject = Properties.Settings.Default.AutoLoadMods;
        List<string> WorkshopMapPaths = new List<string>();
        string ParkPFile = "Labs_DoubleGoal_V2_P.upk";

        // Import c++ inject function
        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            RLMenuBarInit();
            GetPaths();
            GetMaps();
            if (IsModuleLoaded("RLModding"))
            {
                injectLbl.Content = "Injected";
            }
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

        /* Get Workshop path and CookedPCConsole path */
        private static void GetPaths()
        {
            if (Properties.Settings.Default.Cooked_Path != String.Empty && Properties.Settings.Default.Workshop_Path != String.Empty)
            {
                return;
            }

            if (Properties.Settings.Default.Cooked_Path == String.Empty && Properties.Settings.Default.Path != String.Empty)
            {
                int startindex = Properties.Settings.Default.Path.IndexOf("rocketleague");
                int endindex = Properties.Settings.Default.Path.IndexOf("\\\\", startindex);
                string sub = Properties.Settings.Default.Path.Substring(0, endindex + 2);
                string cooked_path = Path.Combine(sub, "TAGame\\\\CookedPCConsole");
                Properties.Settings.Default.Cooked_Path = cooked_path;
            }
            if (Properties.Settings.Default.Workshop_Path == String.Empty && Properties.Settings.Default.Path != String.Empty)
            {
                int startindex = Properties.Settings.Default.Path.IndexOf("steamapps");
                int endindex = Properties.Settings.Default.Path.IndexOf("\\\\", startindex);
                string sub = Properties.Settings.Default.Path.Substring(0, endindex + 2);
                string workshop_path = Path.Combine(sub, "workshop\\\\content\\\\252950");
                Properties.Settings.Default.Workshop_Path = workshop_path;
            }
            Properties.Settings.Default.Save();
        }

        private void GetMaps()
        {
            if (Properties.Settings.Default.Cooked_Path == String.Empty || Properties.Settings.Default.Workshop_Path == String.Empty)
            {
                return;
            }

            workshop_maps_combo.Items.Clear();
            foreach(var folder in Directory.GetDirectories(Properties.Settings.Default.Workshop_Path))
            {
                foreach(var file in Directory.GetFiles(folder, "*.udk", SearchOption.AllDirectories))
                {
                    workshop_maps_combo.Items.Add(Path.GetFileNameWithoutExtension(file));
                    WorkshopMapPaths.Add(file);
                }
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
                injectWatcher = new Thread(() => AutoLoadMods());
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
                if (Process.GetProcessesByName("RocketLeague").Length < 1)
                {
                    // Change inject label using delegate callback
                    updateLabel delUpadte = new updateLabel(UpdateUI);
                    this.injectLbl.Dispatcher.BeginInvoke(delUpadte, "Not Injected");

                }
                else if (IsModuleLoaded("RLModding.dll"))
                {
                    // Change inject label using delegate callback
                    updateLabel delUpadte = new updateLabel(UpdateUI);
                    this.injectLbl.Dispatcher.BeginInvoke(delUpadte, "Injected");
                }
                else if (auto_inject && !injectWatcher.IsAlive)
                {
                    Console.WriteLine("Starting auto inject after close");
                    injectWatcher = new Thread(() => AutoLoadMods());
                    injectWatcher.IsBackground = true;
                    injectWatcher.Start();
                }
                Thread.Sleep(3000);
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
                            GetPaths();
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
            if (!IsModuleLoaded("RLModding.dll"))
            {
                if (Inject())
                {
                    System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                    sound.Play();
                    injectLbl.Content = "Injected";
                }
                
            }
              
        }

        /* Check if dll is injected */
        private static bool IsModuleLoaded(String ModuleName)
        {
            if (Process.GetProcessesByName("RocketLeague").Length > 0)
            {
                try
                {
                    var q = from p in Process.GetProcessesByName("RocketLeague")
                            from m in p.Modules.OfType<ProcessModule>()
                            select m;
                    return q.Any(pm => pm.ModuleName.Contains(ModuleName));
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            
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
                injectWatcher = new Thread(() => AutoLoadMods());
                injectWatcher.IsBackground = true;
                injectWatcher.Start();
                auto_inject = true;
            }
            else if (!btnAutoLoadMods.IsChecked)
            {
                Console.WriteLine("Aborting AutoLoad thread");
                abort = true;
                auto_inject = false;
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
                MessageBox.Show("WinPcap not detected, please download it in order to use direct ip");
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

        private void refresh_maps_Click(object sender, RoutedEventArgs e)
        {
            GetPaths();
            GetMaps();
            //Thow this in here to enable 'Rocket League' menu bar
            RLMenuBarInit();
        }

        private void restore_map_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Workshop_Path != String.Empty && Properties.Settings.Default.Workshop_Path != String.Empty)
            {

                string cooked_path = Properties.Settings.Default.Cooked_Path;
                string parkp_path = Path.Combine(cooked_path, ParkPFile);
                try
                {
                    string backed_up = ParkPFile + ".bak";
                    if (File.Exists(Path.Combine(cooked_path, backed_up)))
                    {
                        File.Delete(parkp_path);
                        //Restore original map
                        File.Copy(Path.Combine(cooked_path, backed_up), parkp_path);
                        swap_label.Content = "Original Double Goals Map Restored Successfully";
                    }
                    else
                    {
                        swap_label.Content = "Original Already Exists";
                    }
                    
                }
                catch (Exception)
                {
                    MessageBox.Show("Please close rocket league to restore the map");
                }
            }
        }

        private void swap_map_Click(object sender, RoutedEventArgs e)
        {
            string ws_path = Properties.Settings.Default.Workshop_Path;
            string cooked_path = Properties.Settings.Default.Cooked_Path;
            string parkp_path = Path.Combine(cooked_path, ParkPFile);
            int index;

            if (workshop_maps_combo.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a workshop map");
                return;
            }
            else
            {
                index = workshop_maps_combo.SelectedIndex;
            }
            

            try
            {
                string backed_up = ParkPFile + ".bak";
                //Back up file
                if (!File.Exists(Path.Combine(cooked_path, backed_up)))
                {
                    File.Copy(parkp_path, Path.Combine(cooked_path, backed_up));
                }
                
                File.Delete(parkp_path);
                string selectedMap = WorkshopMapPaths[index];
                File.Copy(selectedMap, parkp_path);
                swap_label.Content = "Swapped Successfully With Double Goals Map";
                
            }
            catch (Exception)
            {
                MessageBox.Show("Please close rocket league to swap maps");
            }

        }
    }
}
