using System;
using System.Text;

namespace Octopus_SmartData
{
    public class Consumption
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public Entry[] results { get; set; }
    }

    public class Entry
    {
        public double consumption { get; set; }
        public DateTime interval_start { get; set; }
        public DateTime interval_end { get; set; }
    }

    public class ConsumptionInfo
    {
        public string Title { get; set; }
        public double TotalConsumption { get; set; }
        public double AverageConsumption { get; set; }
        public double Percentage { get; set; }
        public double TotalCost { get; set; }
        public double AverageCost { get; set; }
        public double ForecastedTotalCost { get; set; }
        public double ForecastedAverageCost { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.Title);
            sb.AppendFormat("Percent consumption     : {0:0.00}%\n", Percentage);
            sb.AppendFormat("Total consumption       : {0:0.00} kWh\n", TotalConsumption);
            sb.AppendFormat("Average consumption     : {0:0.00} kWh\n", AverageConsumption);
            sb.AppendFormat("Total cost              : {0:0.00} GBP\n", TotalCost);
            sb.AppendFormat("Average cost            : {0:0.00} GBP\n", AverageCost);
            sb.AppendFormat("Forecasted Total cost   : {0:0.00} GBP\n", ForecastedTotalCost);
            sb.AppendFormat("Forecasted Average cost : {0:0.00} GBP\n", ForecastedAverageCost);

            return sb.ToString();
        }
    }
}