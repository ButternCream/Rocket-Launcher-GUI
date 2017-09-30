using System.Diagnostics;
using System.Windows;

namespace RocketLauncher_GUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Notice : Window
    {
        public Notice()
        {
            InitializeComponent();
        }

        private void dwnldWinpCap_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.winpcap.org/install/default.htm");
            this.Close();
        }
    }
}
