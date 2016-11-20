using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net;
using System.Threading;

using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using AgWe_CSharp_Client.Data;
using AgWe_CSharp_Client.Data.Readings;
using AgWe_CSharp_Client.Charts;

namespace AgWe_CSharp_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ManualResetEvent ManualResetEvent = null;
        private Queue<object> events;
        private Socket socket;
        private AgWeDao dao = new AgWeDao();

        public SeriesCollection TempCollection { get; set; }
        public SeriesCollection HumidCollection { get; set; }
        public SeriesCollection LightCollection { get; set; }
        public Func<double, string> Formatter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            dao.InitializeDB();
            var readings = new Readings();
            TempCollection = new TimeLineSeries("Temp", readings.TempReadings).collection;
            HumidCollection = new TimeLineSeries("Humid", readings.HumidReadings).collection;
            LightCollection = new TimeLineSeries("Light", readings.LightReadings).collection;
            Formatter = value => new System.DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("t");
            DataContext = this;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            IPAddress address = ValidateIP(txtIPAddress.Text);

            if (address == null)
            {
                txtIPAddress.Text = "Invalid IP";
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
                RegisterListeners(socket);
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, () =>
            {
                lblStatusOut.Dispatcher.Invoke(
                    new UpdateContentCallback(this.UpdateContent),
                    new object[] {"Connection Error", lblStatusOut}
                );
            });

        }

        

        private void RegisterListeners (Socket socket)
        {
            socket.On("temp", (data) =>
            {
                ReceiveData("temp", data);
            });

            socket.On("humid", (data) =>
            {
                ReceiveData("humid", data);
            });

            socket.On("light", (data) =>
            {
                ReceiveData("light", data);
            });
        }

        private void ReceiveData(string reading, object data)
        {
            events.Enqueue(data);
            ManualResetEvent.Set();
            ManualResetEvent.WaitOne();

            var obj = (JObject)events.Dequeue();
            var str = (string)obj[reading];

            if (reading.Equals("temp"))
            {
                this.Dispatcher.Invoke(
                    new UpdateGraph(this.updateGraph),
                    new object[] {str, TempCollection}
                );
            }
            if (reading.Equals("humid"))
            {
                this.Dispatcher.Invoke(
                    new UpdateGraph(this.updateGraph),
                    new object[] { str, HumidCollection }
                );
            }
            if (reading.Equals("light"))
            {
                this.Dispatcher.Invoke(
                    new UpdateGraph(this.updateGraph),
                    new object[] { str, LightCollection }
                );
            }

            if ( dao.SaveReading("100", reading, str) )
            {
                Console.WriteLine("Reading Saved");
            }
            else
            {
                Console.WriteLine("Save Error");
            }

        }

        private IPAddress ValidateIP(string ip)
        {
            IPAddress address;
            dao.Read();
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

        private void updateGraph(string temp, SeriesCollection graph)
        {
            graph[0].Values.Add(new ReadingModel { DateTime = DateTime.Now, Value = Convert.ToDouble(temp) });
        }

        public delegate void UpdateGraph(string message, SeriesCollection graph);
    }
}
