﻿
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
            string url = "";
            string apiKey = "";

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

            var dailyConsumptions = await dataManager.FetchDailyUsageData(url, apiKey, 30, offPeakStart, offPeakEnd);

            foreach (var consumption in dailyConsumptions)
            {
                Console.WriteLine(consumption.ToString());
            }

            var consumptions = dataManager.FetchUsageData(
                dailyConsumptions,
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
