using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace AsyncPattern
{
    public class Graph
    {
        public void Draw(HashSet<Measurement> measurements)
        {
            var chart = new Chart() {
                dataProvider = new List<DataProvider>()
            };

            var log = measurements.OrderBy(x => x.ElapsedMs).ToList();

            log.ForEach(x => {
                var categoryName = x.TaskName
                    .Replace("start", "")
                    .Replace("end", "")
                    .Trim();

                var timeInMs = ((double)x.ElapsedMs / (double)100).ToString();

                var dataCell = chart.dataProvider.FirstOrDefault(
                    x => x.category == categoryName);

                if (dataCell == null)
                {
                    chart.dataProvider.Add(new DataProvider()
                    {
                        category = categoryName,
                        open = x.TaskName.IndexOf("start") > -1 ? timeInMs : "",
                        close = x.TaskName.IndexOf("end") > -1 ? timeInMs : ""
                    });
                }
                else
                {
                    if (x.TaskName.IndexOf("start") > -1)
                    {
                        dataCell.open = timeInMs;
                    }

                    if (x.TaskName.IndexOf("end") > -1)
                    {
                        dataCell.close = timeInMs;
                    }
                }
            });

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
