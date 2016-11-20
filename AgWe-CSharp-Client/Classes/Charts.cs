using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using AgWe_CSharp_Client.Data.Readings;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

namespace AgWe_CSharp_Client.Charts
{
    class TimeLineSeries : SeriesCollection
    {
        public SeriesCollection collection;
        public TimeLineSeries (string title, List<ReadingModel> readings)
        {
            var dayConfig = Mappers.Xy<ReadingModel>()
                .X(ReadingModel => (double)ReadingModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(ReadingModel => ReadingModel.Value);
            this.collection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = title,
                    Values = new ChartValues<ReadingModel>(readings),
                    Fill = Brushes.Transparent,
                    PointGeometry = null
                }
            };
        }
    }
}
