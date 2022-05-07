
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus_SmartData;

namespace Octopus_OffPeak_Calc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string url = "https://api.octopus.energy/v1/electricity-meter-points/1013040206969/meters/21L3824437/consumption/";
            string apiKey = "sk_live_hHxfb4AG03X9SENp7oT8fWSy";

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Please ensure that consumption url and apiKey are valid");
                return;
            }

            int durationInDays = 30;

            var currentOffPeakRate = 0.055;
            var currentPeakRate = 0.1412;

            var newOffPeakRate = 0.08;
            var newPeakRate = 0.3083;

            TimeSpan offPeakStart = new TimeSpan(20, 30, 0);
            TimeSpan offPeakEnd = new TimeSpan(1, 30, 0);

            var dataManager = new DataManager();

            var consumptions = await dataManager.FetchUsageData(
                url, 
                apiKey, 
                durationInDays, 
                currentPeakRate, 
                currentOffPeakRate, 
                newPeakRate, 
                newOffPeakRate, 
                offPeakStart, 
                offPeakEnd);

            foreach(var consumption in consumptions)
            {
                Console.WriteLine(consumption.ToString());
            }
        }
    }
}
