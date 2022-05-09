using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Octopus_SmartData
{
    public class DataManager
    {
        public async Task<List<DailyConsumption>> FetchDailyUsageData(
            string url,
            string apiKey,
            int durationInDays,
            TimeSpan offPeakStart,
            TimeSpan offPeakEnd
            )
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<HttpClient>();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(apiKey)));

            var yesterday = DateTime.Today.AddDays(-1);
            var lastDate = yesterday.AddDays(-1 * durationInDays);

            bool continueProcessing = true;

            List<TimeSpan> offPeakTimes = new List<TimeSpan>();
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var hour = i;
                    var minute = j * 30;
                    var timeSpan = new TimeSpan(hour, minute, 0);

                    if (offPeakStart < offPeakEnd)
                    {
                        if (timeSpan >= offPeakStart && timeSpan < offPeakEnd)
                        {
                            offPeakTimes.Add(timeSpan);
                        }
                    }
                    else
                    {
                        if (timeSpan >= offPeakStart || timeSpan < offPeakEnd)
                        {
                            offPeakTimes.Add(timeSpan);
                        }
                    }
                }
            }

            Dictionary<DateTime, DailyConsumption> dailyConsumption = new Dictionary<DateTime, DailyConsumption>();

            Dictionary<TimeSpan, double> usageDictionary = new Dictionary<TimeSpan, double>();

            while (continueProcessing)
            {
                if (string.IsNullOrEmpty(url))
                {
                    break;
                }

                var response = await client.GetFromJsonAsync<Consumption>(url);

                if (response.results.Length == 0)
                    break;

                for (var i = 0; i < response.results.Length; i++)
                {
                    var entry = response.results[i];
                    if (entry.interval_start < lastDate)
                    {
                        continueProcessing = false;
                        break;
                    }

                    if (entry.interval_start.Date > yesterday)
                    {
                        continue;
                    }

                    var date = entry.interval_start.Date;
                    var timeOfDay = entry.interval_start.TimeOfDay;

                    if (!dailyConsumption.ContainsKey(date))
                    {
                        dailyConsumption[date] = new DailyConsumption() { Date = date };
                    }

                    var consumption = dailyConsumption[date];

                    if (offPeakTimes.Contains(timeOfDay))
                    {
                        consumption.OffPeak += entry.consumption;
                    }
                    else
                    {
                        consumption.Peak += entry.consumption;
                    }
                }

                url = response.next;
            }

            var list = dailyConsumption.Values.ToList();

            list.OrderByDescending(x => x.Date.DayOfYear);

            return list;
        }

        public List<ConsumptionInfo> FetchUsageData(
            List<DailyConsumption> dailyConsumptions,
            double currentPeakRate, 
            double currentOffPeakRate, 
            double newPeakRate,
            double newOffPeakRate,
            TimeSpan offPeakStart,
            TimeSpan offPeakEnd
            )
        {
            double totalUsage = 0;
            double peakUsage = 0;
            double offPeakUsage = 0;

            foreach(var dailyConsumption in dailyConsumptions)
            {
                peakUsage += dailyConsumption.Peak;
                offPeakUsage += dailyConsumption.OffPeak;
            }

            totalUsage = peakUsage + offPeakUsage;

            var durationInDays = dailyConsumptions.Count;

            List<ConsumptionInfo> consumptions = new List<ConsumptionInfo>();
            
            ConsumptionInfo peakConsumption = new ConsumptionInfo()
            {
                Title = "Peak",
                TotalConsumption = peakUsage,
                AverageConsumption = peakUsage / durationInDays,
                Percentage = peakUsage / totalUsage * 100,
                TotalCost = peakUsage * currentPeakRate,
                AverageCost = peakUsage * currentPeakRate / durationInDays,
                ForecastedTotalCost = peakUsage * newPeakRate,
                ForecastedAverageCost = peakUsage * newPeakRate / durationInDays,
            };
            consumptions.Add(peakConsumption);

            ConsumptionInfo offPeakConsumption = new ConsumptionInfo()
            {
                Title = "Off Peak",
                TotalConsumption = offPeakUsage,
                AverageConsumption = offPeakUsage / durationInDays,
                Percentage = offPeakUsage / totalUsage * 100,
                TotalCost = offPeakUsage * currentOffPeakRate,
                AverageCost = offPeakUsage * currentOffPeakRate / durationInDays,
                ForecastedTotalCost = offPeakUsage * newOffPeakRate,
                ForecastedAverageCost = offPeakUsage * newOffPeakRate / durationInDays,
            };
            consumptions.Add(offPeakConsumption);

            ConsumptionInfo totalConsumption = new ConsumptionInfo()
            {
                Title = "Overall",
                TotalConsumption = peakUsage + offPeakUsage,
                AverageConsumption = peakConsumption.AverageConsumption + offPeakConsumption.AverageConsumption,
                Percentage = 100,
                TotalCost = peakConsumption.TotalCost + offPeakConsumption.TotalCost,
                AverageCost = peakConsumption.AverageCost + offPeakConsumption.AverageCost,
                ForecastedTotalCost = peakConsumption.ForecastedTotalCost + offPeakConsumption.ForecastedTotalCost,
                ForecastedAverageCost = peakConsumption.ForecastedAverageCost + offPeakConsumption.ForecastedAverageCost,
            };
            consumptions.Add(totalConsumption);


            return consumptions;
        }
    }
}
