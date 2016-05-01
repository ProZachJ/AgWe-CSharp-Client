using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace AgWe_CSharp_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            IPAddress address = ValidateIP(txtIPAddress.Text);

            if (address == null)
            {
                txtIPAddress.Text = "ERROR";
                return;
            }


        }

        private IPAddress ValidateIP(string ip)
        {
            IPAddress address;

            IPAddress.TryParse(ip, out address);

            return address;
        }
    }
}
