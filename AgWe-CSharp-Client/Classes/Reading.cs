using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using AgWe_CSharp_Client.Data;

namespace AgWe_CSharp_Client.Data.Readings
{
    public class ReadingModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; } 
        public string kind { get; set; }
    }

    public class Readings
    {
        public List<ReadingModel> TempReadings = new List<ReadingModel>();
        public List<ReadingModel> HumidReadings = new List<ReadingModel>();
        public List<ReadingModel> LightReadings = new List<ReadingModel>();
        public List<ReadingModel> readings = new List<ReadingModel>();


        public Readings()
        {
            AgWeDao dao = new AgWeDao();
            readings = dao.getLastDay();
            readings.ForEach(populateReadings);
        }
        private void populateReadings(ReadingModel item)
        {
            if (item.kind.Equals("temp"))
            {
                this.TempReadings.Add(item);
            }
            if (item.kind.Equals("humid"))
            {
                this.HumidReadings.Add(item);
            }
            if (item.kind.Equals("light"))
            {
                this.LightReadings.Add(item);
            }
        }
    }
    

}
