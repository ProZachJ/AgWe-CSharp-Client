using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using Quobject.SocketIoClientDotNet.Client;
using System.Threading;
using Newtonsoft.Json.Linq;

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

        private ManualResetEvent ManualResetEvent = null;
        private Queue<object> events;
        private Socket socket;
        
        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            IPAddress address = ValidateIP(txtIPAddress.Text);

            if (address == null)
            {
                txtIPAddress.Text = "ERROR";
                return;
            }

            ManualResetEvent = new ManualResetEvent(false);
            events = new Queue<object>();

            socket = IO.Socket(CreateUri(address));

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                lblStatusOut.Dispatcher.Invoke(
                    new UpdateContentCallback(this.UpdateContent),
                    new object[] {"Connected", lblStatusOut}
                );
            });
            
            socket.On("temp", (data) =>
            {
                events.Enqueue(data);
                ManualResetEvent.Set();
                ReceiveData("temp", lblTempOut);
            });

            

            socket.On("humid", (data) =>
            {
                events.Enqueue(data);
                ManualResetEvent.Set();
                ReceiveData("humid", lblHumidityOut);
            });

            socket.On("light", (data) =>
            {
                events.Enqueue(data);
                ManualResetEvent.Set();
                ReceiveData("light", lblLightOut);
            });

        }

        private void ReceiveData(string reading, Label label)
        {
            ManualResetEvent.WaitOne();
            var obj = (JObject)events.Dequeue();
            var str = (string)obj[reading];
            label.Dispatcher.Invoke(
                new UpdateContentCallback(this.UpdateContent),
                new object[] { str, label }
            );
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

        private void UpdateContent(string message, Label label)
        {
            label.Content = message;
        }

        public delegate void UpdateContentCallback(string message, Label label);
    }
}
