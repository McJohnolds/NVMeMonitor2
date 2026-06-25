using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Media;
using LibreHardwareMonitor.Hardware;

namespace NVMeMonitor2
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private Computer _computer;

        // --- ALARM VARIABLES ---
        private const double DEFAULT_ALARM_TEMP = 70.0;
        private const double MP700_ALARM_TEMP = 72.0;
        private const double HEALTH_WARNING_LIMIT = 90.0;
        private bool _flashToggle = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;

            _computer = new Computer { IsStorageEnabled = true };
            _computer.Open();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += UpdateTemperatures;
            _timer.Start();

            UpdateTemperatures(null, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Top + (desktopWorkingArea.Height - this.ActualHeight) / 2;
        }

        private void UpdateTemperatures(object? sender, EventArgs? e)
        {
            try
            {
                int driveCount = 0;

                TempText.Inlines.Clear();
                TempText.Text = "";

                foreach (IHardware hardware in _computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Storage)
                    {
                        driveCount++;
                        hardware.Update();

                        double? temp = null;
                        double? remainingLife = null;
                        bool hasCriticalWarning = false;

                        foreach (ISensor sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature && temp == null)
                            {
                                temp = sensor.Value;
                            }
                            else if (sensor.Name.Contains("Remaining Life", StringComparison.OrdinalIgnoreCase))
                            {
                                remainingLife = sensor.Value;
                            }
                            // Fixed: Explicitly look for "Critical Warning" to avoid false positives 
                            // from "Warning Temperature" threshold sensors.
                            else if (sensor.Name.Equals("Critical Warning", StringComparison.OrdinalIgnoreCase) && sensor.Value > 0)
                            {
                                hasCriticalWarning = true;
                            }
                        }

                        // --- 1. TEMPERATURE DISPLAY ---
                        double activeThreshold = hardware.Name.Contains("MP700", StringComparison.OrdinalIgnoreCase)
                                                 ? MP700_ALARM_TEMP : DEFAULT_ALARM_TEMP;

                        if (temp.HasValue)
                        {
                            double currentTemp = Math.Round(temp.Value);
                            Run driveText = new Run($"{hardware.Name}: {currentTemp}°C\n");

                            if (currentTemp >= activeThreshold)
                            {
                                driveText.Foreground = _flashToggle ? Brushes.Red : Brushes.White;
                            }
                            else
                            {
                                driveText.Foreground = Brushes.White;
                            }

                            TempText.Inlines.Add(driveText);
                        }
                        else
                        {
                            TempText.Inlines.Add(new Run($"{hardware.Name}: Temp Sensor Blocked\n") { Foreground = Brushes.DarkGray });
                        }

                        // --- 2. S.M.A.R.T. DISPLAY (HIDDEN BY DEFAULT) ---
                        bool showSmartAlert = false;
                        string alertMessage = "";

                        // Check for physical faults
                        if (hasCriticalWarning)
                        {
                            showSmartAlert = true;
                            alertMessage += "[CRITICAL FAULT] ";
                        }

                        // Check for degraded health
                        if (remainingLife.HasValue && remainingLife.Value < HEALTH_WARNING_LIMIT)
                        {
                            showSmartAlert = true;
                            alertMessage += $"[LOW LIFE: {Math.Round(remainingLife.Value)}%] ";
                        }

                        // ONLY inject this line into the UI if something is actually wrong
                        if (showSmartAlert)
                        {
                            Run smartText = new Run($"   └ {alertMessage.Trim()}\n");
                            smartText.FontSize = 12;
                            smartText.Foreground = Brushes.Yellow;
                            TempText.Inlines.Add(smartText);
                        }

                        // Add spacing between drives
                        TempText.Inlines.Add(new Run("\n"));
                    }
                }

                if (driveCount == 0)
                {
                    TempText.Text = "0 Storage Devices Detected.\nCheck Admin Rights.";
                }

                _flashToggle = !_flashToggle;
            }
            catch (Exception)
            {
                TempText.Text = "Fatal Sensor Error";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _computer.Close();
            base.OnClosed(e);
        }
    }
}