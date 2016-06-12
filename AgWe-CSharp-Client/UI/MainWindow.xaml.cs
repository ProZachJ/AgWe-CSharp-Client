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
using Quobject.SocketIoClientDotNet.Client;

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

        private Socket socket;
        
        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            IPAddress address = ValidateIP(txtIPAddress.Text);

            if (address == null)
            {
                txtIPAddress.Text = "ERROR";
                return;
            }

            socket = IO.Socket(CreateUri(address));
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                lblOut.Dispatcher.Invoke(
                    new UpdateContentCallback(this.UpdateContent),
                    new object[] {"Connected"}
                );
            });
     
        }

        private IPAddress ValidateIP(string ip)
        {
            IPAddress address;

            IPAddress.TryParse(ip, out address);
            return address;
        }

        private string CreateUri(IPAddress address)
        {
            var uri = string.Format("http://{0}", address.ToString());
            return uri;
        }

        private void UpdateContent (string message)
        {
            lblOut.Content = message;
        }

        public delegate void UpdateContentCallback(string message);
    }
}
