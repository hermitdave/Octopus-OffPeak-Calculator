
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Octopus_OffPeak_Calc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string url = "";
            string apiKey = "";

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Please ensure that consumption url and apiKey are valid");
                return;
            }

            int durationInDays = 30;

            var currentOffPeakRate = 5.5;
            var currentPeakRate = 14.12;

            var newOffPeakRate = 7.5;
            var newPeakRate = 30.83;

            var services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<HttpClient>();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(apiKey)));

            var yesterday = DateTime.Today.AddDays(-1);
            var lastDate = yesterday.AddDays(-1*durationInDays);

            bool continueProcessing = true;

            Dictionary<TimeSpan, double> usageDictionary = new Dictionary<TimeSpan, double>();

            double totalUsage = 0;
            double offPeakUsage = 0;
            TimeSpan peakStart = new TimeSpan(1, 30, 0);
            TimeSpan peakEnd = new TimeSpan(20, 30, 0);

            while (continueProcessing)
            {
                var response = await client.GetFromJsonAsync<Consumption>(url);

                if (response.results.Length == 0)
                    break;

                for(var i = 0; i < response.results.Length; i++)
                {
                    var entry = response.results[i];
                    if (entry.interval_start < lastDate)
                    {
                        continueProcessing = false;
                        break;
                    }

                    var timeOfDay = entry.interval_start.TimeOfDay;

                    if (!usageDictionary.ContainsKey(timeOfDay))
                    {
                        usageDictionary[timeOfDay] = 0;
                    }
                    usageDictionary[timeOfDay] += entry.consumption;

                    totalUsage += entry.consumption;

                    if (timeOfDay >= peakStart && timeOfDay < peakEnd)
                    {
                        // peak hour - do whatever is needed
                    }
                    else
                    {
                        offPeakUsage += entry.consumption;
                    }
                }

                url = response.next;
            }



            //foreach(var key in usageDictionary.Keys)
            //{
            //    Console.WriteLine($"Usage at {key} was {usageDictionary[key]}");
            //}
            Console.WriteLine($"Electricity consumption kWh in last {durationInDays} days");
            Console.WriteLine();

            Console.WriteLine($"Off peak consumption: {Math.Round(offPeakUsage)}");
            Console.WriteLine($"Total consumption: {Math.Round(totalUsage)}");
            Console.WriteLine();

            double peakUsage = totalUsage - offPeakUsage;

            var dailyPeakUsage = (totalUsage - offPeakUsage) / durationInDays;
            var dailyOffPeakUsage = offPeakUsage / durationInDays;
            var dailyUsage = totalUsage / durationInDays;

            var currentOffPeakCharge = offPeakUsage * currentOffPeakRate / 100;
            var currentPeakCharge = peakUsage * currentPeakRate / 100;
            var currentCharge = currentPeakCharge + currentOffPeakCharge;

            var newOffPeakCharge = offPeakUsage * newOffPeakRate / 100;
            var newPeakCharge = peakUsage * newPeakRate / 100;
            var newCharge = newPeakCharge + newOffPeakCharge;

            Console.WriteLine($"Average daily consumption is {Math.Round(dailyUsage)}");
            Console.WriteLine($"Average daily peak consumption is {Math.Round(dailyPeakUsage)}");
            Console.WriteLine($"Average daily off peak consumption is {Math.Round(dailyOffPeakUsage)}");


            Console.WriteLine();
            Console.WriteLine($"Curent 30 day bill\nPeak: {Math.Round(currentPeakCharge)} + offPeakUsage: {Math.Round(currentOffPeakCharge)} is {Math.Round(currentCharge)}");
            Console.WriteLine();
            Console.WriteLine($"Forecasted 30 day bill\nPeak: {Math.Round(newPeakCharge)} + offPeakUsage: {Math.Round(newOffPeakCharge)} is {Math.Round(newCharge)}");
        }
    }
}
