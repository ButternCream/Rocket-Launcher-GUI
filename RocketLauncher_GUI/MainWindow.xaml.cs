using PcapDotNet.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        Thread procWatcher = new Thread(new ThreadStart(GetProcInfo));   
        Thread injectWatcher = new Thread(new ThreadStart(AutoLoadMods));
        volatile static bool abort = false;
        volatile static bool injected = false;

        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            RLMenuBarInit();
            //GetDevices();
        }

        private void GetDevices()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            try
            {
                foreach (var d in LivePacketDevice.AllLocalMachine)
                {
                    string desc = d.Description;
                    int f = desc.IndexOf('\'', desc.IndexOf('\'') + 1);
                    Console.WriteLine(f);
                    list.Add(d.Description.Remove(f).Replace("'", ""));
                }
                deviceCombo.ItemsSource = list;
                if (Properties.Settings.Default.DeviceIndex > -1)
                {
                    deviceCombo.SelectedIndex = Properties.Settings.Default.DeviceIndex;
                }
            } catch(FileNotFoundException)
            {
                return;
            }
            
         
        }

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

        private void LoadSettings()
        {
            btnAutoLoadMods.IsChecked = Properties.Settings.Default.AutoLoadMods;
            if (btnAutoLoadMods.IsChecked && !injectWatcher.IsAlive)
            {
                Console.WriteLine("Started AutoLoad thread");
                abort = false;
                injectWatcher = new Thread(new ThreadStart(AutoLoadMods));
                injectWatcher.IsBackground = true;
                injectWatcher.Start();
            }
            if (Properties.Settings.Default.Path == String.Empty)
            {
                procWatcher.IsBackground = true;
                procWatcher.Start();
            }
            
        }

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
                        Console.WriteLine("Found rocket league for auto injection thread");
                        Thread.Sleep(20000);
                        if (Inject())
                            injected = true;
                        else
                            injected = false;
                    }
                    else { Thread.Sleep(1000); }
                }
                Console.WriteLine("Exiting Thread");
            }
        }

         private void btnDonate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XHLHQGAQK2XZG");
        }

        

        private void btnLoadMods_Click(object sender, RoutedEventArgs e)
        {
            if (!injected && Inject())
                injected = true;
            else
                injected = false;
        }

       

        private void btnAutoLoadMods_Clicked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoLoadMods = btnAutoLoadMods.IsChecked;
            Properties.Settings.Default.Save();
            if (btnAutoLoadMods.IsChecked && !injectWatcher.IsAlive)
            {
                Console.WriteLine("Started AutoLoad thread");
                abort = false;
                injectWatcher = new Thread(new ThreadStart(AutoLoadMods));
                injectWatcher.Start();
            }
            else if (!btnAutoLoadMods.IsChecked)
            {
                Console.WriteLine("Aborting AutoLoad thread");
                abort = true;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            String RL = Properties.Settings.Default.Path + "RocketLeague.exe";
            Process.Start(RL);
        }

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("RocketLeague");
            if (procs.Length > 0)
            {
                procs[0].Kill();
            }
        }

        private void deviceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.DeviceIndex = deviceCombo.SelectedIndex;
        }

        private void InterceptDev(object sender, RoutedEventArgs e)
        {
            Support_Files.LAN.serverList.Add(IP.Text);
            if (Properties.Settings.Default.DeviceIndex > -1)
            {
                Properties.Settings.Default.Save();

                Thread packets = new Thread(() => Support_Files.LAN.Intercept(Properties.Settings.Default.DeviceIndex));
                packets.IsBackground = true;
                packets.Start();
                Console.WriteLine("Packet thread started!");
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
