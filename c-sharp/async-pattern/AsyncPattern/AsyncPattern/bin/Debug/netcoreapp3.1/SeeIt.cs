using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace AsyncPattern
{
    public class SeeIt
    {
        private Chart chart;

        public SeeIt()
        {
            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "chart.json");

            var j = File.ReadAllText(path);

            chart = JsonConvert.DeserializeObject<Chart>(j);
        }

        public void Draw(string taskName, long atMs)
        {
            var which = new Dictionary<string, string>() {
                { "API call 1 start",  "API call 1"},
                { "API call 1 end",  "API call 1"},

                { "API call 2 start",  "API call 2"},
                { "API call 2 end",  "API call 2"},

                { "API call 3 start",  "API call 3"},
                { "API call 3 end",  "API call 3"},

                { "Process result 1 start",  "Process result 1"},
                { "Process result 1 end",  "Process result 1"},

                { "Process result 2 start",  "Process result 2"},
                { "Process result 2 end",  "Process result 2"},

                { "Process result 3 start",  "Process result 3"},
                { "Process result 3 end",  "Process result 3"},
            };

            if (which.TryGetValue(taskName, out string v) == false)
            {
                return;
            }

            var d = chart.dataProvider.FirstOrDefault(d =>
                d.category == v);

            if (d == null)
            {
                return;
            }

            var ms = ((double)atMs / (double)100).ToString();

            if (taskName.IndexOf("start") > -1)
            {
                d.open = ms;
            }

            if (taskName.IndexOf("end") > -1)
            {
                d.close = ms;
            }   
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(chart);

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"draw.json");

            File.WriteAllText(path, json);
        }
    }

    public class Chart
    {
        public List<DataProvider> dataProvider { get; set; }
    }

    public class DataProvider
    {
        public string category { get; set; }
        public string open { get; set; }
        public string close { get; set; }
    }
}
