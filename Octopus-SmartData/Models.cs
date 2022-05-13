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

    public class DailyConsumption
    {
        public DateTime Date { get; set; }
        public double Peak { get; set; }
        public double OffPeak { get; set; }

        public double OffPeakPercentage
        {
            get
            {
                return OffPeak * 100 / Total;
            }
        }

        public String OffPeakPercentDisplay
        {
            get
            {
                return OffPeakPercentage.ToString("N0");
            }
        }

        public double Total
        {
            get
            {
                return Peak + OffPeak;
            }
        }

        public override string ToString()
        {

            return String.Format("Date: {0}, Peak: {1:0.00}kWh, Off Peak: {2:0.00}", Date, Peak, OffPeak);
        }

        public string PeakDisplay
        {
            get
            {
                return String.Format("{0:0.00}", Peak);
            }
        }

        public string OffPeakDisplay
        {
            get
            {
                return String.Format("{0:0.00}", OffPeak);
            }
        }

        public string TotalDisplay
        {
            get
            {
                return String.Format("{0:0.00}", Total);
            }
        }

        public PartConsumption PeakConsumption
        {
            get
            {
                return GetPartConsumption(this.Peak);
            }
        }


        public PartConsumption OffPeakConsumption
        {
            get
            {
                return GetPartConsumption(this.OffPeak);
            }
        }

        private PartConsumption GetPartConsumption(double value)
        {
            return new PartConsumption
            {
                Date = this.Date,
                Consumption = value
            };
        }

        public string CurrentCost(double peakRate, double offPeakRate)
        {
            return GetCostInfo(Peak, OffPeak, peakRate, offPeakRate);
        }

        public string NewCost(double peakRate, double offPeakRate)
        {
            return GetCostInfo(Peak, OffPeak, peakRate, offPeakRate);
        }

        public string GetCostInfo(double peakUsage, double offPeakUsage, double peakRate, double offPeakRate)
        {
            var peakCost = peakUsage * peakRate;
            var offPeakCost = offPeakUsage * offPeakRate;

            var cost = peakCost + offPeakCost;

            return $"{peakCost.ToString("N2")} + {offPeakCost.ToString("N2")} = {cost.ToString("N2")}";
        }
    }

    public class PartConsumption
    {
        public DateTime Date { get; set; }
        public double Consumption {get; set;}
        public string DisplayValue
        {
            get
            {
                return Consumption.ToString("N2");
            }
        }
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

        public double AverageCostPerkWh
        {
            get
            {
                return TotalCost * 100 / TotalConsumption;
            }
        }

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
            sb.AppendFormat("Average rate per kWh    : {0:0.00} p\n", AverageCostPerkWh);

            return sb.ToString();
        }
    }
}