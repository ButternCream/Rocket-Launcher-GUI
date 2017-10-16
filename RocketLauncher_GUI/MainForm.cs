using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketLauncher_GUI
{
    public partial class MainForm : Form
    {
        // Import c++ inject function
        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject();
        [DllImport("Injector.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Inject_Beta();

        //form scope variables
        private bool isDllInjected = false;
        private bool dllInjectionVerified = false;
        private DateTime injectionStartTime;
        private LogFile logger;
        private bool initialized = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //ensure our CurrentDirectory is our executing assembly location
            //this will correct errors caused by shorcuts and whatnot
            Environment.CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            //set title
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += $" {assemblyVersion.Major}.{assemblyVersion.Minor}";

            //check if we're already running and close if we are
            var mutex = new Mutex(true, "RocketLauncher", out var mutexCreated);
            if (!mutexCreated)
            {
                logger.WriteLine("Mutex already created. Shutting down. (An instance of Rocket Launcher is already running)");

                mutex.Close();
                Application.Exit();
                return;
            }

            //create logger and start new log
            logger = new LogFile("RL.log");
            logger.Clear();
            logger.WriteLine($"Initializing {this.Text}");

            //verify installation
            bool installationCorrect = true;
            if (!File.Exists("Injector.dll"))
                installationCorrect = false;

            //yell at the user for doing things wrong
            if (!installationCorrect)
            {
                MessageBox.Show(
                    "One or more required program files are missing. Please fully extract Rocket Launcher and try again.",
                    "Invalid Installation", MessageBoxButtons.OK, MessageBoxIcon.Error);


                //log details before exiting
                logger.WriteLine("Oh no! The Injector doesn't exist. Lets see some details that will help us diagnose how this happened");
                logger.WriteLine($"The executing assembly is located in the directory {new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Name}");
                logger.WriteLine($"This directory contains {Directory.GetFiles(Environment.CurrentDirectory, "*.*", SearchOption.TopDirectoryOnly).Length - 1} other files");
                logger.WriteLine("Well, we can't function without the injector, so that's all folks!");

                Application.Exit();
                return;
            }

            //startup actions
            LoadSettings();
            RLProcessMonitor.LookForProcess(); //do a game running check
            CheckForUpdates(); //check for an update
            initialized = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save settings
            if(initialized)
            {
                logger.WriteLine("Saving settings");
                Properties.Settings.Default.Save();
            }
        }

        #region Utility functions

        private bool IsInjected()
        {
            if (RLProcessMonitor.RocketLeagueProcess == null)
                return false;

            var modules = RLProcessMonitor.RocketLeagueProcess.Modules;
            foreach (ProcessModule module in modules)
            {
                if (module.ModuleName == "RLModding.dll")
                {
                    return true;
                }
            }
            return false;
        }
        private void LoadSettings()
        {
            //this function is needed because menu strip items are
            //stupid and keep generating CheckState properties
            autoLoadModsToolStripMenuItem.Checked = Properties.Settings.Default.AutoLoadMods;
            playSoundOnInjectToolStripMenuItem.Checked = Properties.Settings.Default.SoundNotif;
            useBetaChannelToolStripMenuItem.Checked = Properties.Settings.Default.UseBetaVersion;
        }

        private void AttemptInjection()
        {
            logger.WriteLine("AttemptInjection()");
            bool injectionResult = useBetaChannelToolStripMenuItem.Checked ? Inject_Beta() : Inject();

            if (!injectionResult)
            {
                logger.WriteLine("Injection failed due to !injectionResult");
                injectStatusLabel.Text = "Injection Failed";
            }
            else
            {
                injectButton.Enabled = false;
                injectStatusLabel.Text = "Injecting...";
                isDllInjected = true;
                injectionStartTime = DateTime.Now;
            }
        }

        private void SetupPaths()
        {
            //check if we already have our workshop and cooked path
            if (!string.IsNullOrEmpty(Properties.Settings.Default.CookedPath) && !string.IsNullOrEmpty(Properties.Settings.Default.WorkshopPath))
            {
                return;
            }

            //check if we have a game apth
            if (string.IsNullOrEmpty(Properties.Settings.Default.GamePath))
            {
                return;
            }

            logger.WriteLine("Setting up workshop and cooked paths");
            //setup cooked path
            int startindex = Properties.Settings.Default.GamePath.IndexOf("rocketleague", StringComparison.CurrentCultureIgnoreCase);
            int endindex = Properties.Settings.Default.GamePath.IndexOf("\\\\", startindex, StringComparison.CurrentCultureIgnoreCase);
            string sub = Properties.Settings.Default.GamePath.Substring(0, endindex + 2);
            string cookedPath = Path.Combine(sub, "TAGame\\\\CookedPCConsole");

            //verify it exists
            if (Directory.Exists(cookedPath))
            {
                Properties.Settings.Default.CookedPath = cookedPath;
            }

            //setup workshop path
            startindex = Properties.Settings.Default.GamePath.IndexOf("steamapps", StringComparison.CurrentCultureIgnoreCase);
            endindex = Properties.Settings.Default.GamePath.IndexOf("\\\\", startindex, StringComparison.CurrentCultureIgnoreCase);
            sub = Properties.Settings.Default.GamePath.Substring(0, endindex + 2);
            string workshopPath = Path.Combine(sub, "workshop\\\\content\\\\252950");

            //verify it exists
            if (Directory.Exists(cookedPath))
            {
                Properties.Settings.Default.WorkshopPath = workshopPath;
            }
        }

        private Dictionary<string, string> workshopMapPaths = new Dictionary<string, string>();
        private void RefreshWorkshopMaps()
        {
            logger.WriteLine("Populating workshop maps");

            //clear combo and dictionary
            mapListComboBox.Items.Clear();
            workshopMapPaths.Clear();
            
            //get udk files
            foreach (var folder in Directory.GetDirectories(Properties.Settings.Default.WorkshopPath))
            {
                foreach (var file in Directory.GetFiles(folder, "*.udk", SearchOption.AllDirectories))
                {
                    string keyName = Path.GetFileNameWithoutExtension(file);
                    mapListComboBox.Items.Add(keyName);
                    workshopMapPaths[keyName] = file;
                }
            }
        }

        // Still kept this for when they check "Use Beta Version"
        private void CheckForDLLUpdate()
        {
            //determine manifest url
            string manifestUrl = useBetaChannelToolStripMenuItem.Checked ?  Properties.Resources.BetaVersionManifestURL : Properties.Resources.ReleaseVersionManifestURL;

            //get new version
            DateTime version;
            try
            {
                version = DateTime.Parse(((new WebClient()).DownloadString(manifestUrl)));
                logger.WriteLine($"CheckForUpdate, found version {version.ToString()}, beta version? {useBetaChannelToolStripMenuItem.Checked.ToString()}");
            }
            catch (Exception e)
            {
                logger.WriteLine("Exception while getting update manifest: " + e.ToString());
                return;
            }

            //check against current version
            DateTime currentVersion = useBetaChannelToolStripMenuItem.Checked ? Properties.Settings.Default.BetaVersion : Properties.Settings.Default.ReleaseVersion;
            string targetDirectory = useBetaChannelToolStripMenuItem.Checked ? "Beta" : "Release";
            string targetPath = targetDirectory + "\\RLModding.dll";
            string targetPathBackup = targetPath + ".old";
            string targetUrl = useBetaChannelToolStripMenuItem.Checked ? Properties.Resources.BetaVersionDLLURL : Properties.Resources.ReleaseVersionDLLURL;

            //check if this is a new update
            if (version <= currentVersion && File.Exists(targetPath))
            {   
                return;
            }  
                
            

            //we have a new update!
            var res = MessageBox.Show($"An update for the {targetDirectory} version has been found. Would you like to download it?", "Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No)
                return;

            //user wants to update
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            //check if we should backup
            if (File.Exists(targetPath))
            {
                //delete old backup if it exists
                if(File.Exists(targetPathBackup))
                    File.Delete(targetPathBackup);
                File.Copy(targetPath, targetPathBackup);
                File.Delete(targetPath);
            }

            //download new
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFileAsync(new Uri(targetUrl), targetPath);
                }
                //update the version
                if (useBetaChannelToolStripMenuItem.Checked)
                {
                    Properties.Settings.Default.BetaVersion = version;
                }
                else
                {
                    Properties.Settings.Default.ReleaseVersion = version;
                }
            } catch (Exception exc)
            {
                logger.WriteLine("Exception during update download: " + exc.ToString());
            }
        }

        // NEW General Updater, basically copy and pasted from previous functions
        private void CheckForUpdates(bool buttonClick = false)
        {
            string gui_manifestUrl = Properties.Resources.GuiManifest_Url;
            string dll_manifestUrl = useBetaChannelToolStripMenuItem.Checked ? Properties.Resources.BetaVersionManifestURL : Properties.Resources.ReleaseVersionManifestURL;
            //get new version
            DateTime gui_version;
            DateTime dll_version;
            try
            {
                gui_version = DateTime.Parse(((new WebClient()).DownloadString(gui_manifestUrl)));
                dll_version = DateTime.Parse(((new WebClient()).DownloadString(dll_manifestUrl)));
                logger.WriteLine($"Update, found version {gui_version.ToString()}");
            }
            catch (Exception e)
            {
                logger.WriteLine("Exception while getting update manifest: " + e.ToString());
                return;
            }

            //Check if there is an update
            DateTime currentGUIVersion = Properties.Settings.Default.GuiVersion;
            DateTime currentDLLVersion = useBetaChannelToolStripMenuItem.Checked ? Properties.Settings.Default.BetaVersion : Properties.Settings.Default.ReleaseVersion;
            bool update_gui = gui_version > currentGUIVersion;
            bool update_dll = dll_version > currentDLLVersion;
            if (update_gui || update_dll)
            {
                var res = MessageBox.Show($"A new version has been found. Would you like to download it?", "Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.No)
                    return;
                // Check if GUI needs updating
                if (update_gui)
                {
                    UpdateGUI();
                    Properties.Settings.Default.GuiVersion = gui_version;
                }
                //Check if DLL needs updating
                if (update_dll)
                {
                    UpdateDLL();
                    if (useBetaChannelToolStripMenuItem.Checked)
                        Properties.Settings.Default.BetaVersion = dll_version;
                    else
                        Properties.Settings.Default.ReleaseVersion = dll_version;
                  
                }

                    
            }
            else
            {
                if (buttonClick)
                    MessageBox.Show("Version up to date.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        // Update the GUI if its out of date
        private void UpdateGUI()
        {
            string batchFile = Properties.Resources.GuiBatchFile_Url;
            string targetUrl = Properties.Resources.Gui_Url;
            string targetDirectory = "temp";
            string targetPath = targetDirectory + "\\Rocket Launcher.exe";

            //user wants to update, create temp directory for new exe
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            //download new
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(new Uri(targetUrl), targetPath);
                }
            }
            catch (Exception exc)
            {
                logger.WriteLine("Exception during update download: " + exc.ToString());
            }

            // Download and run updater.bat which,
            // Closes instance of Rocket Launcher (if running)
            // Creates temp directory and downloads the new exe
            // Copies and overwrites the old exe
            // Deletes temp directory
            // Starts Rocket Launcher
            // Deletes itself
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(new Uri(batchFile), "updater.bat");
                }
                Process.Start("updater.bat");

            }
            catch (Exception exc)
            {
                logger.WriteLine("Exception during update download: " + exc.ToString());
            }
        }

        // Update the DLL(s) if they're out of date, depending on if they are using beta or not
        private void UpdateDLL()
        {

            //check against current version
            string targetDirectory = useBetaChannelToolStripMenuItem.Checked ? "Beta" : "Release";
            string targetPath = targetDirectory + "\\RLModding.dll";
            string targetPathBackup = targetPath + ".old";
            string targetUrl = useBetaChannelToolStripMenuItem.Checked ? Properties.Resources.BetaVersionDLLURL : Properties.Resources.ReleaseVersionDLLURL;

            //user wants to update
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            //check if we should backup
            if (File.Exists(targetPath))
            {
                //delete old backup if it exists
                if (File.Exists(targetPathBackup))
                    File.Delete(targetPathBackup);
                File.Copy(targetPath, targetPathBackup);
                File.Delete(targetPath);
            }

            //download new
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFileAsync(new Uri(targetUrl), targetPath);
                }  
            }
            catch (Exception exc)
            {
                logger.WriteLine("Exception during update download: " + exc.ToString());
            }
        }

        #endregion

        #region Timer Tick Events
        private void processExistsTimer_Tick(object sender, EventArgs e)
        {
            RLProcessMonitor.LookForProcess();

            //if dll is injected and our process becomes null, reset UI state
            if (RLProcessMonitor.RocketLeagueProcess == null && isDllInjected)
            {
                injectButton.Enabled = true;
                dllInjectionVerified = false;
                isDllInjected = false;
                injectStatusLabel.Text = "Not Injected";
            }

            //save path if it's not saved already
            if (RLProcessMonitor.RocketLeagueProcess != null &&
                string.IsNullOrEmpty((Properties.Settings.Default.GamePath)))
            {
                string path = RLProcessMonitor.RocketLeagueProcess.MainModule.FileName.Replace("\\", "\\\\");
                Properties.Settings.Default.GamePath = path.Remove(path.Length - 16);

                //setup cooked and workshop paths
                SetupPaths();
                RefreshWorkshopMaps();
            }
        }


        private void injectionVerifier_Tick(object sender, EventArgs e)
        {
            if (!isDllInjected)
                return;
            if (dllInjectionVerified)
                return;
            if (RLProcessMonitor.RocketLeagueProcess == null)
                return;

            //check if we've been trying for too long
            if ((DateTime.Now - injectionStartTime).Seconds > 10)
            {
                logger.WriteLine("Injection failed due to timeout");
                injectButton.Enabled = true;
                isDllInjected = false;
                injectStatusLabel.Text = "Injection Failed";
            }

            //check if injection succeeded
            if (IsInjected())
            {
                logger.WriteLine("Injection succeeded");
                dllInjectionVerified = true;
                injectStatusLabel.Text = "Injected";

                //play sound
                if (playSoundOnInjectToolStripMenuItem.Checked && File.Exists(@"C:\Windows\Media\chimes.wav"))
                {
                    System.Media.SoundPlayer sound = new System.Media.SoundPlayer(@"C:\Windows\Media\chimes.wav");
                    sound.Play();
                }

            }
        }

        private void autoLoadModsTimer_Tick(object sender, EventArgs e)
        {
            if (isDllInjected)
                return;

            //attempt to inject
            if (RLProcessMonitor.RocketLeagueProcess != null)
            {
                var startTimeSpan = DateTime.Now - RLProcessMonitor.RocketLeagueProcess.StartTime;
                if (startTimeSpan.Seconds > 10)
                {
                    AttemptInjection();
                }
            }
        }
        #endregion

        #region Main TabPage Control Events
        private void button1_Click(object sender, EventArgs e)
        {
            if (RLProcessMonitor.RocketLeagueProcess == null)
            {
                MessageBox.Show("Please run the game before attempting to load mods.", "Injection Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //check if injected already
            if (IsInjected())
            {
                injectButton.Enabled = false;
                injectStatusLabel.Text = "Injected";
                dllInjectionVerified = true;
                isDllInjected = true;
                return;
            }

            //run injection process
            AttemptInjection();
        }
        #endregion

        #region Direct IP Control Events
        private void downloadPcapButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.winpcap.org/install/bin/WinPcap_4_1_3.exe");
        }

        private void runDirectIpButton_Click(object sender, EventArgs e)
        {
            //verify IP address
            if (!Regex.IsMatch(ipTextbox.Text, @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}$"))
            {
                MessageBox.Show("Please enter a valid IP address.", "Invalid IP Address", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            //attempt to run!
            try
            {
                List<string> serverList = new List<string>() { ipTextbox.Text + ":7777" };
                Support_Files.Simulator.Intercept(serverList);
                MessageBox.Show("Running. Go to 'Play > Play Local > Join Local Lobby' in Rocket League.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //update button
                runDirectIpButton.Enabled = false;
                runDirectIpButton.Text = "Running...";

                //update text box
                ipTextbox.Enabled = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error occurred trying to intercept Rocket League. Make sure of the following:\n1) WinPcap is installed\n2) This application has been run as an administrator", "Interception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.WriteLine("Exception during intercept: " + exc.ToString());
            }
        }
        #endregion

        #region Map Swap Control Events
        private void mapListRefreshButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.WorkshopPath))
            {
                MessageBox.Show("The workshop path is not set yet. Please run Rocket League in order to set this.", "Error refreshing map list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RefreshWorkshopMaps();
        }

        private void mapSwapButton_Click(object sender, EventArgs e)
        {
            if (mapListComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("You must select a map to swap first!", string.Empty, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                string underpassPath = Path.Combine(Properties.Settings.Default.CookedPath, Properties.Resources.UnderpassFilename);
                string underpassBackupPath = underpassPath + ".bak";

                //Back up file
                if (!File.Exists(underpassBackupPath))
                {
                    File.Copy(underpassPath, underpassBackupPath);
                    logger.WriteLine("Backed up Underpass");
                }

                //delete old underpass
                File.Delete(underpassPath);

                //copy new underpass
                string selectedMapPath = workshopMapPaths[mapListComboBox.Text];
                File.Copy(selectedMapPath, underpassPath);

                //notify the user
                MessageBox.Show($"Map swap success! Underpass should now load up {mapListComboBox.Text}.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                logger.WriteLine($"Swapped Underpass with {mapListComboBox.Text}");

            }
            catch (Exception exc)
            {
                MessageBox.Show("An exception occurred while attempting to perform a map swap. Please close Rocket League when attempting to perform a map swap.", "Map Swap Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.WriteLine("Exception during map swap: " + exc.ToString());
            }
        }

        private void mapRestoreButton_Click(object sender, EventArgs e)
        {
            //get paths
            string underpassPath = Path.Combine(Properties.Settings.Default.CookedPath, Properties.Resources.UnderpassFilename);
            string underpassBakPath = underpassPath + ".bak";

            //check if bak file exists
            if (!File.Exists(underpassBakPath))
            {
                MessageBox.Show("Could not restore Underpass as no maps have been swapped yet.", "Restoration Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //swap
            try
            {
                File.Delete(underpassPath);
                File.Copy(underpassBakPath, underpassPath);
                MessageBox.Show("Underpass has been restored to it's original form!", "Restore Successful",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                File.Delete(underpassBakPath);
            }
            catch (Exception exc)
            {
                MessageBox.Show("An exception occured during the restoration process. Please close Rocket League and try again.", "Restoration Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.WriteLine("Exception: " + exc.ToString());
            }

        }
        #endregion

        #region Other Menu Buttons
        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XHLHQGAQK2XZG");
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        private void workshopTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://rocketleaguemods.com/mods/workshop-textures/");
        }

        private void uModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://storage.googleapis.com/google-code-archive-downloads/v2/code.google.com/texmod/uMod_alpha_v2_r49.zip");
        }
        #endregion

        #region "Settings" menu
        private void playSoundOnInjectToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SoundNotif = playSoundOnInjectToolStripMenuItem.Checked;
        }

        private void useBetaChannelToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseBetaVersion = useBetaChannelToolStripMenuItem.Checked;

            //notify the user
            if (isDllInjected)
            {
                MessageBox.Show(
                    "Version changes will only be applied the next time you use \"Load Mods\". Restart Rocket League if you want to apply the version change.",
                    "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //call updater
            CheckForDLLUpdate();
        }

        private void autoLoadModsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            autoLoadModsTimer.Enabled = autoLoadModsToolStripMenuItem.Checked;
            Properties.Settings.Default.AutoLoadMods = autoLoadModsToolStripMenuItem.Checked;
            
        }
        #endregion

        #region "Rocket League" menu
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("steam://rungameid/252950");
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RLProcessMonitor.RocketLeagueProcess != null)
                RLProcessMonitor.RocketLeagueProcess.Kill();
        }
        #endregion
    }
}
