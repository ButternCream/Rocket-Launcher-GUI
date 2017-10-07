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
        Dictionary<string,string> WorkshopMapPaths = new Dictionary<string, string>();
        string UnderpassFile = "Labs_Underpass_P.upk";


        // Import c++ inject function
        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject();
        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject_Beta();

        public MainWindow()
        {
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                Process.GetCurrentProcess().Kill();
            ClearLog();
            Logger("Initing Program");
            InitializeComponent();
            if (!Properties.Settings.Default.NoNotifyUpdate)
                CheckForUpdates();
            Logger("Loading Settings");
            LoadSettings();
            Logger("Getting paths");
            GetPaths();
            Logger("Getting maps");
            GetMaps();
            Logger("Checking if injected");
            if (IsModuleLoaded("RLModding.dll"))
            {
                injectLbl.Content = "Injected";
            }
            Logger("Starting RL check thread");
            //Start thread to check if RL is running / if dll is injected
            updateThread = new Thread(new ThreadStart(CheckForRL));
            updateThread.Name = "Update Thread";
            updateThread.IsBackground = true;
            updateThread.Start();
        }

        private static void ClearLog()
        {
            StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + "/RL.log", false);
            file.Write(String.Empty);
            file.Close();
        }

        public static void Logger(String lines)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            try
            {
                StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + "/RL.log", true);
                file.WriteLine(lines);

                file.Close();
            }
            catch (Exception)
            {

            }
            

        }

        /* Get Workshop path and CookedPCConsole path */
        private static void GetPaths()
        {
            if (Properties.Settings.Default.Cooked_Path != String.Empty && Properties.Settings.Default.Workshop_Path != String.Empty)
            {
                return;
            }
            Logger("Has CookedPC path and Workshop path");
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
            Logger("Saving CookedPC path and Workshop path");
            Properties.Settings.Default.Save();
        }

        private void GetMaps()
        {
            if (Properties.Settings.Default.Cooked_Path == String.Empty || Properties.Settings.Default.Workshop_Path == String.Empty)
            {
                return;
            }
            Logger("Populating workshop maps");
            workshop_maps_combo.Items.Clear();
            foreach(var folder in Directory.GetDirectories(Properties.Settings.Default.Workshop_Path))
            {
                foreach(var file in Directory.GetFiles(folder, "*.udk", SearchOption.AllDirectories))
                {
                    workshop_maps_combo.Items.Add(Path.GetFileNameWithoutExtension(file));
                    if (!WorkshopMapPaths.ContainsKey(Path.GetFileNameWithoutExtension(file)))
                    {
                        WorkshopMapPaths.Add(Path.GetFileNameWithoutExtension(file), file);
                    }
                      
                }
            }
        }

        /*
         * Start auto injector if enabled and process watcher if path is empty
         */
        private void LoadSettings()
        {
            btnAutoLoadMods.IsChecked = Properties.Settings.Default.AutoLoadMods;
            btnSound.IsChecked = Properties.Settings.Default.Sound;
            btnBeta.IsChecked = Properties.Settings.Default.IsBeta;
            if (btnAutoLoadMods.IsChecked && !IsModuleLoaded("RLModding.dll"))
            {
                Logger("Started AutoLoad thread");
                abort = false;
                injectWatcher = new Thread(() => AutoLoadMods());
                injectWatcher.IsBackground = true;
                injectWatcher.Start();
            }
            if (Properties.Settings.Default.Path == String.Empty)
            {
                Logger("Starting thread to get proc info");
                procWatcher = new Thread(new ThreadStart(GetProcInfo));
                procWatcher.IsBackground = true;
                procWatcher.Start();
            }
            if (Properties.Settings.Default.Map_Index >= 0)
            {
                workshop_maps_combo.SelectedIndex = Properties.Settings.Default.Map_Index;
            }
            
        }

        /*
         * Check if RL is running and if not, update the injector label
         */
        private void CheckForRL()
        {
            Logger("In CheckForRL");
            while (true)
            {
                if (Process.GetProcessesByName("RocketLeague").Length < 1)
                {
                    Logger("Rocket League not running. updating injection status");
                    // Change inject label using delegate callback
                    updateLabel delUpadte = new updateLabel(UpdateUI);
                    this.injectLbl.Dispatcher.BeginInvoke(delUpadte, "Not Injected");

                }
                else if (IsModuleLoaded("RLModding.dll"))
                {
                    Logger("Module injected. updating injection status");
                    // Change inject label using delegate callback
                    updateLabel delUpadte = new updateLabel(UpdateUI);
                    this.injectLbl.Dispatcher.BeginInvoke(delUpadte, "Injected");
                }
                else if (auto_inject && !injectWatcher.IsAlive)
                {
                    Logger("Starting auto inject via CheckForRL");
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
                            Logger("Found path! " + Properties.Settings.Default.Path);
                            GetPaths();
                            return;

                        }
                    } catch (Exception e)
                    {
                        Logger("Exception: " + e.ToString());
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
                        
                        Thread.Sleep(6000);
                        if (!IsModuleLoaded("RLModding.dll"))
                        {
                            Logger("Injecting");
                            
                            injected = Properties.Settings.Default.IsBeta ? Inject_Beta() : Inject();
                            if (Properties.Settings.Default.Sound && injected)
                            {
                                System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                                sound.Play();
                            }
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
                if (Properties.Settings.Default.IsBeta && Inject_Beta())
                {
                    
                    if (Properties.Settings.Default.Sound)
                    {
                        System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                        sound.Play();
                    }
                    injectLbl.Content = "Injected";
                    Logger("Injected Beta DLL");
                }
                else if (Inject())
                {
                    if (Properties.Settings.Default.Sound)
                    {
                        System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                        sound.Play();
                    }
                    injectLbl.Content = "Injected";
                    Logger("Injected Release DLL");
                }
                else
                {
                    Logger("Couldn't Inject");
                }

            }
              
        }

        /* Check if dll is injected */
        private static bool IsModuleLoaded(String ModuleName)
        {
            Logger("In isModuleLoaded");
            if (Process.GetProcessesByName("RocketLeague").Length > 0)
            {
                try
                {
                    var q = from p in Process.GetProcessesByName("RocketLeague")
                            from m in p.Modules.OfType<ProcessModule>()
                            select m;
                    return q.Any(pm => pm.ModuleName.Contains(ModuleName));
                }
                catch (Exception e)
                {
                    Logger("Exception: " + e.ToString());
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
                Logger("Started AutoLoad thread");
                abort = false;
                injectWatcher = new Thread(() => AutoLoadMods());
                injectWatcher.IsBackground = true;
                injectWatcher.Start();
                auto_inject = true;
            }
            else if (!btnAutoLoadMods.IsChecked)
            {
                Logger("Abort set for AutoLoad thread");
                abort = true;
                auto_inject = false;
            }
        }

        /* Start RL */
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("steam://rungameid/252950");
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
                MessageBox.Show("Running. Go to 'join local match' in RL");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Make sure you have\n1) Installed WinPcap\n2)Run this program as admin");
                Logger("Exception: " + exc.ToString());
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
            catch (Exception exc)
            {
                Logger("Exception: " + exc.ToString());
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
            Logger("Refreshing maps");
            GetPaths();
            GetMaps();
            if (Properties.Settings.Default.Map_Index >= 0)
            {
                workshop_maps_combo.SelectedIndex = Properties.Settings.Default.Map_Index;
            }
        }

        private void restore_map_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Workshop_Path != String.Empty && Properties.Settings.Default.Workshop_Path != String.Empty)
            {

                string cooked_path = Properties.Settings.Default.Cooked_Path;
                string underpass_path = Path.Combine(cooked_path, UnderpassFile);
                try
                {
                    string backed_up = UnderpassFile + ".bak";
                    if (File.Exists(Path.Combine(cooked_path, backed_up)))
                    {
                        File.Delete(underpass_path);
                        //Restore original map
                        File.Copy(Path.Combine(cooked_path, backed_up), underpass_path);
                        swap_label.Content = "Original Underpass Restored Successfully";
                        Logger("Original Underpass Restored Successfully");
                    }
                    else
                    {
                        swap_label.Content = "Original Already Exists";
                        Logger("Original Already Exists");
                    }
                    
                }
                catch (Exception exc)
                {
                    swap_label.Content = "Unable To Restore. Please Close Rocket League";
                    Logger("Exception: " + exc.ToString());
                }
            }
        }

        private void swap_map_Click(object sender, RoutedEventArgs e)
        {
            string ws_path = Properties.Settings.Default.Workshop_Path;
            string cooked_path = Properties.Settings.Default.Cooked_Path;
            string underpass_path = Path.Combine(cooked_path, UnderpassFile);

            if (workshop_maps_combo.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a workshop map");
                return;
            }
            

            try
            {
                string backed_up = UnderpassFile + ".bak";
                //Back up file
                if (!File.Exists(Path.Combine(cooked_path, backed_up)))
                {
                    File.Copy(underpass_path, Path.Combine(cooked_path, backed_up));
                    Logger("Backed up underpass");
                }
                
                File.Delete(underpass_path);
                string selectedMap = WorkshopMapPaths[workshop_maps_combo.Text];
                File.Copy(selectedMap, underpass_path);
                swap_label.Content = "Swapped Successfully With Underpass";
                Logger("Swapped with underpass");

            }
            catch (Exception exc)
            {
                MessageBox.Show("Please close rocket league to swap maps");
                Logger("Exception: " + exc.ToString());
            }

        }

        private void save_index(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.Map_Index = workshop_maps_combo.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void delete_pcap(object sender, CancelEventArgs e)
        {
            if (File.Exists("WinPcap.exe"))
            {
                Logger("Deleting WinPCap download");
                File.Delete("WinPcap.exe");
            }
        }

        private void btnCheck_For_Update(object sender, RoutedEventArgs e)
        {
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            if ((Properties.Settings.Default.IsBeta && CheckForUpdatedBetaDLL()) || CheckForUpdatedReleaseDLL())
            {
                Properties.Settings.Default.NoNotifyUpdate = false;
                Properties.Settings.Default.Save();
            }       
        }     

        private void btnSound_Clicked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Sound = btnSound.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void btnBeta_Clicked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsBeta = btnBeta.IsChecked;
            Properties.Settings.Default.Save();
            CheckForUpdatedBetaDLL();
        }

        private bool CheckForUpdatedBetaDLL()
        {
            var mainfest_url = "https://hack.fyi/rlmods/beta/version.txt";
            DateTime version;
            try
            {
                version = DateTime.Parse(((new WebClient()).DownloadString(mainfest_url)));
                Logger(version.ToString());
            }
            catch (Exception e)
            {
                Logger("Exception: " + e.ToString());
                return false;
            }
            if (version > Properties.Settings.Default.BetaVersion || !File.Exists(Directory.GetCurrentDirectory() + "/Beta/RLModding.dll"))
            {
                var res = System.Windows.MessageBox.Show("A beta update has been found. Would you like to download it?", "Update", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    DownloadBetaDLL();
                    Properties.Settings.Default.BetaVersion = version;
                    Properties.Settings.Default.Save();
                    return true;
                }
                else
                {
                    Properties.Settings.Default.NoNotifyUpdate = true;
                    Properties.Settings.Default.Save();
                    return false;
                }
            }
            return false;
        }

        private bool CheckForUpdatedReleaseDLL()
        {
            var mainfest_url = "https://hack.fyi/rlmods/release/version.txt";
            DateTime version;
            try
            {
                version = DateTime.Parse(((new WebClient()).DownloadString(mainfest_url)));
                Logger(version.ToString());
            }
            catch (Exception e)
            {
                Logger("Exception: " + e.ToString());
                return false;
            }
            
            if (version > Properties.Settings.Default.ReleaseVersion || !File.Exists(Directory.GetCurrentDirectory() + "/Release/RLModding.dll"))
            {
                var res = System.Windows.MessageBox.Show("A new DLL has been found. Would you like to download it?", "Update", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    DownloadReleaseDLL();
                    Properties.Settings.Default.ReleaseVersion = version;
                    Properties.Settings.Default.Save();
                    return true;
                }
                else
                {
                    Properties.Settings.Default.NoNotifyUpdate = true;
                    Properties.Settings.Default.Save();
                    return false;
                }            
            }
            return false;
        }

        private void DownloadReleaseDLL()
        {
            string beta_file = Directory.GetCurrentDirectory() + "/Release/RLModding.dll";
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Release"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Release");
            }
            Uri dll = new Uri("https://hack.fyi/rlmods/beta/RLModding.dll");
            if (File.Exists(beta_file))
            {
                if (File.Exists(beta_file + ".old"))
                {
                    File.Delete(beta_file + ".old");
                }
                File.Copy(beta_file, beta_file + ".old");

            }
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFileAsync(uri, beta_file);
            }
            catch (Exception exc)
            {
                Logger("Exception: " + exc.ToString());
                
            }
        }

        private void DownloadBetaDLL()
        {
            string beta_file = Directory.GetCurrentDirectory() + "/Beta/RLModding.dll";
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Beta"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Beta");
            }
            Uri dll = new Uri("https://hack.fyi/rlmods/beta/RLModding_Beta.dll");
            if (File.Exists(beta_file))
            {
                if (File.Exists(beta_file + ".old"))
                {
                    File.Delete(beta_file + ".old");
                }
                File.Copy(beta_file, beta_file + ".old");

            }
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFileAsync(uri, beta_file);
            } catch (Exception exc)
            {
                Logger("Exception: " + exc.ToString());
;            }
            
            

        }
    }
}
