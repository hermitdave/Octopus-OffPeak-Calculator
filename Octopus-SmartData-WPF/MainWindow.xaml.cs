using Octopus_SmartData;
using System;
using System.Windows;

namespace Octopus_SmartData_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var strApiKey = this.tbApiKey.Text;
            if (string.IsNullOrWhiteSpace(strApiKey))
            {
                ShowErrorMessage("API Key is not valid");
                this.tbApiKey.Focus();
                return;
            }

            var strDataUrl = this.tbUrl.Text;
            if (string.IsNullOrWhiteSpace(strDataUrl))
            {
                ShowErrorMessage("Consumption Url is not valid");
                this.tbUrl.Focus();
                return;
            }

            var strDuration = this.lbDuration.Text;
            if (string.IsNullOrWhiteSpace(strDuration) || !IsNumber(strDuration))
            {
                ShowErrorMessage("Select report duration");
                this.lbDuration.Focus();
                return;
            }

            var strOffPeakStartHour = this.cbStartHour.Text;
            if (string.IsNullOrWhiteSpace(strOffPeakStartHour) || !IsNumber(strOffPeakStartHour))
            {
                ShowErrorMessage("Select Off Peak start hour");
                this.cbStartHour.Focus();
                return;
            }

            var strOffPeakStartMinute = this.cbStartMinute.Text;
            if (string.IsNullOrWhiteSpace(strOffPeakStartMinute) || !IsNumber(strOffPeakStartMinute))
            {
                ShowErrorMessage("Select Off Peak start minute");
                this.cbStartMinute.Focus();
                return;
            }

            var strOffPeakEndHour = this.cbEndHour.Text;
            if (string.IsNullOrWhiteSpace(strOffPeakEndHour) || !IsNumber(strOffPeakEndHour))
            {
                ShowErrorMessage("Select Off Peak end hour");
                this.cbEndHour.Focus();
                return;
            }

            var strOffPeakEndMinute = this.cbEndMinute.Text;
            if (string.IsNullOrWhiteSpace(strOffPeakEndMinute) || !IsNumber(strOffPeakEndMinute))
            {
                ShowErrorMessage("Select Off Peak end minute");
                this.cbEndMinute.Focus();
                return;
            }

            var strPeakRate = this.tbPeakRate.Text;
            if (string.IsNullOrWhiteSpace(strPeakRate) || !IsNumber(strPeakRate))
            {
                ShowErrorMessage("Enter peak rate in pounds");
                this.tbPeakRate.Focus();
                return;
            }

            var strOffPeakRate = this.tbOffPeakRate.Text;
            if (string.IsNullOrWhiteSpace(strOffPeakRate) || !IsNumber(strOffPeakRate))
            {
                ShowErrorMessage("Enter off peak rate in pounds");
                this.tbOffPeakRate.Focus();
                return;
            }

            var strPeakRateNew = this.tbNewPeakRate.Text;
            if (string.IsNullOrWhiteSpace(strPeakRateNew) || !IsNumber(strPeakRateNew))
            {
                ShowErrorMessage("Enter new peak rate in pounds");
                this.tbNewPeakRate.Focus();
                return;
            }

            var strOffPeakRateNew = this.tbNewOffPeakRate.Text;
            if (string.IsNullOrWhiteSpace(strOffPeakRateNew) || !IsNumber(strOffPeakRateNew))
            {
                ShowErrorMessage("Enter new off peak rate in pounds");
                this.tbNewOffPeakRate.Focus();
                return;
            }

            this.btnProcess.IsEnabled = false;

            var apiKey = strApiKey.Trim();
            var url = strDataUrl.Trim();
            var duration = int.Parse(strDuration);

            var offPeakStartHour = int.Parse(strOffPeakStartHour.Trim());
            var offPeakStartMin = int.Parse(strOffPeakStartMinute.Trim());

            var offPeakEndHour = int.Parse(strOffPeakEndHour.Trim());
            var offPeakEndMin = int.Parse(strOffPeakEndMinute.Trim());

            var currentPeakRate = double.Parse(strPeakRate.Trim());
            var currentOffPeakRate = double.Parse(strOffPeakRate.Trim());
            var newPeakRate = double.Parse(strPeakRateNew.Trim());
            var newOffPeakRate = double.Parse(strOffPeakRateNew.Trim());

            var offPeakStart = new TimeSpan(offPeakStartHour, offPeakStartMin, 0);
            var offPeakEnd = new TimeSpan(offPeakEndHour, offPeakEndMin, 0);

            var dataManager = new DataManager();
            var consumptions = await dataManager.FetchUsageData(
                url,
                apiKey,
                duration,
                currentPeakRate,
                currentOffPeakRate,
                newPeakRate,
                newOffPeakRate,
                offPeakStart,
                offPeakEnd);

            this.dgSmartData.ItemsSource = consumptions;

            this.btnProcess.IsEnabled = true;
        }

        private bool IsNumber(string input)
        {
            return double.TryParse(input, out double val);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
