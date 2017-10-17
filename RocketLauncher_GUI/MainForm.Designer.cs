namespace RocketLauncher_GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.rocketLeagueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoLoadModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSoundOnInjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dLLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useBetaChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miscStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.workshopTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.injectStatusLabel = new System.Windows.Forms.Label();
            this.injectButton = new System.Windows.Forms.Button();
            this.creditsLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.ipTextbox = new System.Windows.Forms.TextBox();
            this.runDirectIpButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.downloadPcapButton = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.mapRestoreButton = new System.Windows.Forms.Button();
            this.mapSwapButton = new System.Windows.Forms.Button();
            this.mapListComboBox = new System.Windows.Forms.ComboBox();
            this.mapListRefreshButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.processExistsTimer = new System.Windows.Forms.Timer(this.components);
            this.injectionVerifierTimer = new System.Windows.Forms.Timer(this.components);
            this.autoLoadModsTimer = new System.Windows.Forms.Timer(this.components);
            this.mainMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rocketLeagueToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.donateToolStripMenuItem,
            this.miscStripMenuItem1});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(330, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // rocketLeagueToolStripMenuItem
            // 
            this.rocketLeagueToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.killToolStripMenuItem});
            this.rocketLeagueToolStripMenuItem.Name = "rocketLeagueToolStripMenuItem";
            this.rocketLeagueToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.rocketLeagueToolStripMenuItem.Text = "Rocket League";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // killToolStripMenuItem
            // 
            this.killToolStripMenuItem.Name = "killToolStripMenuItem";
            this.killToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.killToolStripMenuItem.Text = "Kill";
            this.killToolStripMenuItem.Click += new System.EventHandler(this.killToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem,
            this.dLLToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoLoadModsToolStripMenuItem,
            this.playSoundOnInjectToolStripMenuItem});
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.generalToolStripMenuItem.Text = "General";
            // 
            // autoLoadModsToolStripMenuItem
            // 
            this.autoLoadModsToolStripMenuItem.CheckOnClick = true;
            this.autoLoadModsToolStripMenuItem.Name = "autoLoadModsToolStripMenuItem";
            this.autoLoadModsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.autoLoadModsToolStripMenuItem.Text = "Auto Load Mods";
            this.autoLoadModsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.autoLoadModsToolStripMenuItem_CheckedChanged);
            // 
            // playSoundOnInjectToolStripMenuItem
            // 
            this.playSoundOnInjectToolStripMenuItem.CheckOnClick = true;
            this.playSoundOnInjectToolStripMenuItem.Name = "playSoundOnInjectToolStripMenuItem";
            this.playSoundOnInjectToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.playSoundOnInjectToolStripMenuItem.Text = "Play Sounds On Inject";
            this.playSoundOnInjectToolStripMenuItem.CheckedChanged += new System.EventHandler(this.playSoundOnInjectToolStripMenuItem_CheckedChanged);
            // 
            // dLLToolStripMenuItem
            // 
            this.dLLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useBetaChannelToolStripMenuItem});
            this.dLLToolStripMenuItem.Name = "dLLToolStripMenuItem";
            this.dLLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dLLToolStripMenuItem.Text = "DLL";
            // 
            // useBetaChannelToolStripMenuItem
            // 
            this.useBetaChannelToolStripMenuItem.CheckOnClick = true;
            this.useBetaChannelToolStripMenuItem.Name = "useBetaChannelToolStripMenuItem";
            this.useBetaChannelToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.useBetaChannelToolStripMenuItem.Text = "Use Beta Version";
            this.useBetaChannelToolStripMenuItem.CheckedChanged += new System.EventHandler(this.useBetaChannelToolStripMenuItem_CheckedChanged);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.donateToolStripMenuItem_Click);
            // 
            // miscStripMenuItem1
            // 
            this.miscStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.workshopTexturesToolStripMenuItem,
            this.uModToolStripMenuItem});
            this.miscStripMenuItem1.Name = "miscStripMenuItem1";
            this.miscStripMenuItem1.Size = new System.Drawing.Size(44, 20);
            this.miscStripMenuItem1.Text = "Misc";
            // 
            // workshopTexturesToolStripMenuItem
            // 
            this.workshopTexturesToolStripMenuItem.Name = "workshopTexturesToolStripMenuItem";
            this.workshopTexturesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.workshopTexturesToolStripMenuItem.Text = "Workshop Textures";
            this.workshopTexturesToolStripMenuItem.Click += new System.EventHandler(this.workshopTexturesToolStripMenuItem_Click);
            // 
            // uModToolStripMenuItem
            // 
            this.uModToolStripMenuItem.Name = "uModToolStripMenuItem";
            this.uModToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.uModToolStripMenuItem.Text = "uMod";
            this.uModToolStripMenuItem.Click += new System.EventHandler(this.uModToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(330, 159);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.injectStatusLabel);
            this.tabPage1.Controls.Add(this.injectButton);
            this.tabPage1.Controls.Add(this.creditsLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(322, 133);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // injectStatusLabel
            // 
            this.injectStatusLabel.Location = new System.Drawing.Point(125, 71);
            this.injectStatusLabel.Name = "injectStatusLabel";
            this.injectStatusLabel.Size = new System.Drawing.Size(100, 18);
            this.injectStatusLabel.TabIndex = 2;
            this.injectStatusLabel.Text = "Not Injected";
            this.injectStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // injectButton
            // 
            this.injectButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.injectButton.Location = new System.Drawing.Point(125, 43);
            this.injectButton.Name = "injectButton";
            this.injectButton.Size = new System.Drawing.Size(100, 25);
            this.injectButton.TabIndex = 1;
            this.injectButton.Text = "Load Mods";
            this.injectButton.UseVisualStyleBackColor = true;
            this.injectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // creditsLabel
            // 
            this.creditsLabel.AutoSize = true;
            this.creditsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.creditsLabel.Location = new System.Drawing.Point(6, 115);
            this.creditsLabel.Name = "creditsLabel";
            this.creditsLabel.Size = new System.Drawing.Size(236, 12);
            this.creditsLabel.TabIndex = 0;
            this.creditsLabel.Text = "Created by ButterandCream, Two, Taylor, Dummiesman";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.ipTextbox);
            this.tabPage2.Controls.Add(this.runDirectIpButton);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.downloadPcapButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(322, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Direct IP";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "IP Address:";
            // 
            // ipTextbox
            // 
            this.ipTextbox.Location = new System.Drawing.Point(75, 39);
            this.ipTextbox.Name = "ipTextbox";
            this.ipTextbox.Size = new System.Drawing.Size(239, 20);
            this.ipTextbox.TabIndex = 3;
            this.ipTextbox.Text = "127.0.0.1";
            // 
            // runDirectIpButton
            // 
            this.runDirectIpButton.Location = new System.Drawing.Point(191, 102);
            this.runDirectIpButton.Name = "runDirectIpButton";
            this.runDirectIpButton.Size = new System.Drawing.Size(123, 25);
            this.runDirectIpButton.TabIndex = 2;
            this.runDirectIpButton.Text = "Run";
            this.runDirectIpButton.UseVisualStyleBackColor = true;
            this.runDirectIpButton.Click += new System.EventHandler(this.runDirectIpButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "* Requires WinPcap";
            // 
            // downloadPcapButton
            // 
            this.downloadPcapButton.Location = new System.Drawing.Point(8, 102);
            this.downloadPcapButton.Name = "downloadPcapButton";
            this.downloadPcapButton.Size = new System.Drawing.Size(123, 25);
            this.downloadPcapButton.TabIndex = 0;
            this.downloadPcapButton.Text = "Download WinPcap";
            this.downloadPcapButton.UseVisualStyleBackColor = true;
            this.downloadPcapButton.Click += new System.EventHandler(this.downloadPcapButton_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.mapRestoreButton);
            this.tabPage3.Controls.Add(this.mapSwapButton);
            this.tabPage3.Controls.Add(this.mapListComboBox);
            this.tabPage3.Controls.Add(this.mapListRefreshButton);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(322, 133);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Map Swapper";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // mapRestoreButton
            // 
            this.mapRestoreButton.Location = new System.Drawing.Point(138, 56);
            this.mapRestoreButton.Name = "mapRestoreButton";
            this.mapRestoreButton.Size = new System.Drawing.Size(95, 23);
            this.mapRestoreButton.TabIndex = 4;
            this.mapRestoreButton.Text = "Restore";
            this.mapRestoreButton.UseVisualStyleBackColor = true;
            this.mapRestoreButton.Click += new System.EventHandler(this.mapRestoreButton_Click);
            // 
            // mapSwapButton
            // 
            this.mapSwapButton.Location = new System.Drawing.Point(12, 56);
            this.mapSwapButton.Name = "mapSwapButton";
            this.mapSwapButton.Size = new System.Drawing.Size(95, 23);
            this.mapSwapButton.TabIndex = 3;
            this.mapSwapButton.Text = "Swap";
            this.mapSwapButton.UseVisualStyleBackColor = true;
            this.mapSwapButton.Click += new System.EventHandler(this.mapSwapButton_Click);
            // 
            // mapListComboBox
            // 
            this.mapListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapListComboBox.FormattingEnabled = true;
            this.mapListComboBox.Location = new System.Drawing.Point(12, 29);
            this.mapListComboBox.Name = "mapListComboBox";
            this.mapListComboBox.Size = new System.Drawing.Size(221, 21);
            this.mapListComboBox.TabIndex = 2;
            // 
            // mapListRefreshButton
            // 
            this.mapListRefreshButton.Location = new System.Drawing.Point(239, 27);
            this.mapListRefreshButton.Name = "mapListRefreshButton";
            this.mapListRefreshButton.Size = new System.Drawing.Size(75, 23);
            this.mapListRefreshButton.TabIndex = 1;
            this.mapListRefreshButton.Text = "Refresh";
            this.mapListRefreshButton.UseVisualStyleBackColor = true;
            this.mapListRefreshButton.Click += new System.EventHandler(this.mapListRefreshButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Credits to Thanrek";
            // 
            // processExistsTimer
            // 
            this.processExistsTimer.Enabled = true;
            this.processExistsTimer.Interval = 1000;
            this.processExistsTimer.Tick += new System.EventHandler(this.processExistsTimer_Tick);
            // 
            // injectionVerifierTimer
            // 
            this.injectionVerifierTimer.Enabled = true;
            this.injectionVerifierTimer.Tick += new System.EventHandler(this.injectionVerifier_Tick);
            // 
            // autoLoadModsTimer
            // 
            this.autoLoadModsTimer.Interval = 5000;
            this.autoLoadModsTimer.Tick += new System.EventHandler(this.autoLoadModsTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 183);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.mainMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Rocket Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem rocketLeagueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button injectButton;
        private System.Windows.Forms.Label creditsLabel;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label injectStatusLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ipTextbox;
        private System.Windows.Forms.Button runDirectIpButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button downloadPcapButton;
        private System.Windows.Forms.Button mapRestoreButton;
        private System.Windows.Forms.Button mapSwapButton;
        private System.Windows.Forms.ComboBox mapListComboBox;
        private System.Windows.Forms.Button mapListRefreshButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killToolStripMenuItem;
        private System.Windows.Forms.Timer processExistsTimer;
        private System.Windows.Forms.Timer injectionVerifierTimer;
        private System.Windows.Forms.Timer autoLoadModsTimer;
        private System.Windows.Forms.ToolStripMenuItem miscStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem workshopTexturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dLLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useBetaChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoLoadModsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSoundOnInjectToolStripMenuItem;
    }
}

