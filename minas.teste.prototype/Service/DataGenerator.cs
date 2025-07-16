using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading; // For System.Threading.Timer
using System.Windows.Forms; // For Control.Invoke

namespace minas.teste.prototype.Service
{
    // EventArgs para passar os dados gerados
    public class DataGeneratedEventArgs : EventArgs
    {
        public Dictionary<string, float> CurrentSensorData { get; }
        public DataGeneratedEventArgs(Dictionary<string, float> currentSensorData)
        {
            CurrentSensorData = currentSensorData;
        }
    }

    public class DataGenerator : IDisposable
    {
        // Global Parameters - Initialized in the average error range
        public float HA1 { get; private set; }
        public float HA2 { get; private set; }
        public float HB1 { get; private set; }
        public float HB2 { get; private set; }
        public float MA1 { get; private set; }
        public float MA2 { get; private set; }
        public float MB1 { get; private set; }
        public float MB2 { get; private set; }
        public int TEM { get; private set; }
        public int ROT { get; private set; } // Primary motor rotation can be maintained
        public float DR1 { get; private set; }
        public float DR2 { get; private set; }
        public float DR3 { get; private set; }
        public float DR4 { get; private set; }

        // Piloting Variables renamed to avoid conflict
        public float P_L1 { get; private set; }
        public float P_L2 { get; private set; }
        public float P_L3 { get; private set; }
        public float P_L4 { get; private set; }
        public float PR1 { get; private set; }
        public float PR2 { get; private set; }
        public float PR3 { get; private set; }
        public float PR4 { get; private set; }
        public float VZ1 { get; private set; }
        public float VZ2 { get; private set; }
        public float VZ3 { get; private set; }
        public float VZ4 { get; private set; }

        private System.Threading.Timer _timer;
        private readonly Random _random = new Random();
        private const int Interval = 1000; // Update interval: 1 second (1000 ms)

        // Event to notify subscribers (e.g., Tela_Bombas) that new data is available
        public event EventHandler<DataGeneratedEventArgs> DataGenerated;

        // For tracking min/max values based on checkbox monitoring
        public Dictionary<string, (float min, float max)> MonitoredMinMaxValues { get; private set; }
        private Dictionary<string, float> _currentData;
        private Dictionary<string, float> _initialValues;

        private int _dataGenerationCount = 0; // Contador para a tendência crescente

        // Dictionary to track if a sensor's min/max should be monitored
        private Dictionary<string, bool> _sensorMonitoringEnabled;

        public DataGenerator()
        {
            InitializeParameters();
            _currentData = new Dictionary<string, float>();
            MonitoredMinMaxValues = new Dictionary<string, (float min, float max)>();
            _sensorMonitoringEnabled = new Dictionary<string, bool>();

            // Initialize _currentData and _initialValues with initial parameter values
            UpdateCurrentDataFromProperties();
            _initialValues = new Dictionary<string, float>(_currentData);

            // Initialize monitoring for all sensors as disabled by default
            foreach (var key in _currentData.Keys)
            {
                _sensorMonitoringEnabled[key] = false;
                MonitoredMinMaxValues[key] = (float.MaxValue, float.MinValue); // Initialize with extreme values
            }
        }

        private void InitializeParameters()
        {
            HA1 = 10.15f; HA2 = 10.25f; HB1 = 11.15f; HB2 = 11.25f;
            MA1 = 20.15f; MA2 = 20.25f; MB1 = 21.15f; MB2 = 21.25f;
            TEM = 70; ROT = 1800;
            DR1 = 15.00f; DR2 = 0.10f; DR3 = 0.10f; DR4 = 0.10f;
            P_L1 = 24.00f; P_L2 = 0.00f; P_L3 = 0.00f; P_L4 = 0.00f;
            PR1 = 175.00f; PR2 = 0.50f; PR3 = 0.00f; PR4 = 0.00f;
            VZ1 = 65.00f; VZ2 = 0.00f; VZ3 = 0.00f; VZ4 = 0.00f;
        }

        private void UpdateCurrentDataFromProperties()
        {
            _currentData["HA1"] = HA1; _currentData["HA2"] = HA2; _currentData["HB1"] = HB1; _currentData["HB2"] = HB2;
            _currentData["MA1"] = MA1; _currentData["MA2"] = MA2; _currentData["MB1"] = MB1; _currentData["MB2"] = MB2;
            _currentData["TEM"] = TEM; _currentData["ROT"] = ROT;
            _currentData["DR1"] = DR1; _currentData["DR2"] = DR2; _currentData["DR3"] = DR3; _currentData["DR4"] = DR4;
            _currentData["P_L1"] = P_L1; _currentData["P_L2"] = P_L2; _currentData["P_L3"] = P_L3; _currentData["P_L4"] = P_L4;
            _currentData["PR1"] = PR1; _currentData["PR2"] = PR2; _currentData["PR3"] = PR3; _currentData["PR4"] = PR4;
            _currentData["VZ1"] = VZ1; _currentData["VZ2"] = VZ2; _currentData["VZ3"] = VZ3; _currentData["VZ4"] = VZ4;
        }

        public void Start()
        {
            if (_timer == null)
            {
                // Timer will call OnTimedEvent every 'interval' milliseconds
                _timer = new System.Threading.Timer(OnTimedEvent, null, 0, Interval);
            }
            else
            {
                _timer.Change(0, Interval); // Start immediately, then repeat every interval
            }
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
        }

        public void Reset()
        {
            Stop();
            _dataGenerationCount = 0;
            InitializeParameters(); // Reset all parameters to their initial values
            UpdateCurrentDataFromProperties(); // Update _currentData based on reset parameters

            // Reset min/max monitored values
            foreach (var key in _sensorMonitoringEnabled.Keys.ToList()) // Use ToList to avoid modification during iteration
            {
                MonitoredMinMaxValues[key] = (float.MaxValue, float.MinValue);
            }

            // Optionally, raise DataGenerated event with reset data
            OnDataGenerated(_currentData);
        }

        private void OnTimedEvent(object state)
        {
            GenerateData();
            OnDataGenerated(_currentData);
        }

        private void GenerateData()
        {
            _dataGenerationCount++;

            // Apply a slight increasing trend to all values over time
            // The trend factor can be adjusted. Here, a small linear increase.
            float trendFactor = _dataGenerationCount * 0.01f; // Adjust this value for desired trend strength

            // HA1: 10.00-10.30 bar (main pressure line)
            HA1 = Constrain(GetIncreasingTrendValue(_initialValues["HA1"], 10.00f, 10.30f, trendFactor), 10.00f, 10.30f);
            // HA2: 10.00-10.30 bar (main pressure line)
            HA2 = Constrain(GetIncreasingTrendValue(_initialValues["HA2"], 10.00f, 10.30f, trendFactor), 10.00f, 10.30f);
            // HB1: 11.00-11.30 bar (piloting pressure)
            HB1 = Constrain(GetIncreasingTrendValue(_initialValues["HB1"], 11.00f, 11.30f, trendFactor), 11.00f, 11.30f);
            // HB2: 11.00-11.30 bar (piloting pressure)
            HB2 = Constrain(GetIncreasingTrendValue(_initialValues["HB2"], 11.00f, 11.30f, trendFactor), 11.00f, 11.30f);

            // MA1: 20.00-20.50 bar (motor pressure)
            MA1 = Constrain(GetIncreasingTrendValue(_initialValues["MA1"], 20.00f, 20.50f, trendFactor), 20.00f, 20.50f);
            // MA2: 20.00-20.50 bar (motor pressure)
            MA2 = Constrain(GetIncreasingTrendValue(_initialValues["MA2"], 20.00f, 20.50f, trendFactor), 20.00f, 20.50f);
            // MB1: 21.00-21.50 bar (motor pressure)
            MB1 = Constrain(GetIncreasingTrendValue(_initialValues["MB1"], 21.00f, 21.50f, trendFactor), 21.00f, 21.50f);
            // MB2: 21.00-21.50 bar (motor pressure)
            MB2 = Constrain(GetIncreasingTrendValue(_initialValues["MB2"], 21.00f, 21.50f, trendFactor), 21.00f, 21.50f);

            // TEM: 65-75 degrees Celsius (temperature, should increase slightly)
            TEM = (int)Constrain(GetIncreasingTrendValue(_initialValues["TEM"], 65f, 75f, trendFactor * 0.5f), 65f, 75f); // Slower trend
            // ROT: 1700-1900 rpm (rotation, can have some variation but also slight increase)
            ROT = (int)Constrain(GetIncreasingTrendValue(_initialValues["ROT"], 1700f, 1900f, trendFactor * 0.8f), 1700f, 1900f);

            // DR1: 10.00-20.00 L/min (GH drain and with large variation)
            DR1 = Constrain(GetIncreasingTrendValue(_initialValues["DR1"], 10.00f, 20.00f, trendFactor), 10.00f, 20.00f);
            DR2 = Constrain(GetIncreasingTrendValue(_initialValues["DR2"], 0.05f, 0.20f, trendFactor * 0.1f), 0.05f, 0.20f);
            DR3 = Constrain(GetIncreasingTrendValue(_initialValues["DR3"], 0.05f, 0.20f, trendFactor * 0.1f), 0.05f, 0.20f);
            DR4 = Constrain(GetIncreasingTrendValue(_initialValues["DR4"], 0.05f, 0.20f, trendFactor * 0.1f), 0.05f, 0.20f);

            // P_L1: 22.00-26.00 bar (Piloting can oscillate more)
            P_L1 = Constrain(GetIncreasingTrendValue(_initialValues["P_L1"], 22.00f, 26.00f, trendFactor * 0.7f), 22.00f, 26.00f);
            P_L2 = Constrain(GetIncreasingTrendValue(_initialValues["P_L2"], 0.00f, 0.50f, trendFactor * 0.1f), 0.00f, 0.50f);
            P_L3 = Constrain(GetIncreasingTrendValue(_initialValues["P_L3"], 0.00f, 0.50f, trendFactor * 0.1f), 0.00f, 0.50f);
            P_L4 = Constrain(GetIncreasingTrendValue(_initialValues["P_L4"], 0.00f, 0.50f, trendFactor * 0.1f), 0.00f, 0.50f);

            // PR1: 150.00-200.00 bar (Unstable output pressure, lower on average)
            PR1 = Constrain(GetIncreasingTrendValue(_initialValues["PR1"], 150.00f, 200.00f, trendFactor * 1.5f), 150.00f, 200.00f); // Stronger trend
            PR2 = Constrain(GetIncreasingTrendValue(_initialValues["PR2"], 0.20f, 0.80f, trendFactor * 0.1f), 0.20f, 0.80f);
            PR3 = Constrain(GetIncreasingTrendValue(_initialValues["PR3"], 0.00f, 0.10f, trendFactor * 0.05f), 0.00f, 0.10f);
            PR4 = Constrain(GetIncreasingTrendValue(_initialValues["PR4"], 0.00f, 0.10f, trendFactor * 0.05f), 0.00f, 0.10f);

            // VZ1: REDUCED flow and with large variation (55-80 L/min)
            float nominal_vz1_at_rot_reduced = 70.0f * ((float)ROT / 1800.0f); // Reduced efficiency
            VZ1 = Constrain(GetIncreasingTrendValue(_initialValues["VZ1"], 55.00f, 80.00f, trendFactor * 1.2f), 55.00f, 80.00f);
            VZ2 = Constrain(GetIncreasingTrendValue(_initialValues["VZ2"], 0.00f, 1.00f, trendFactor * 0.1f), 0.00f, 1.00f);
            VZ3 = Constrain(GetIncreasingTrendValue(_initialValues["VZ3"], 0.00f, 1.00f, trendFactor * 0.1f), 0.00f, 1.00f);
            VZ4 = Constrain(GetIncreasingTrendValue(_initialValues["VZ4"], 0.00f, 1.00f, trendFactor * 0.1f), 0.00f, 1.00f);

            // Update _currentData after generating new values
            UpdateCurrentDataFromProperties();

            // Update min/max values if monitoring is enabled for the sensor
            foreach (var entry in _currentData)
            {
                if (_sensorMonitoringEnabled.TryGetValue(entry.Key, out bool enabled) && enabled)
                {
                    var (min, max) = MonitoredMinMaxValues[entry.Key];
                    MonitoredMinMaxValues[entry.Key] = (Math.Min(min, entry.Value), Math.Max(max, entry.Value));
                }
            }
        }

        private float GetIncreasingTrendValue(float initialValue, float minBound, float maxBound, float trendAmount)
        {
            float noise = GetRandomFloat(-0.5f, 0.5f); // Smaller noise for a clearer trend
            float targetValue = initialValue + trendAmount + noise;
            return Constrain(targetValue, minBound, maxBound);
        }

        private float GetRandomFloat(float min, float max)
        {
            return (float)(_random.NextDouble() * (max - min) + min);
        }

        private float Constrain(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        protected virtual void OnDataGenerated(Dictionary<string, float> currentData)
        {
            DataGenerated?.Invoke(this, new DataGeneratedEventArgs(new Dictionary<string, float>(currentData)));
        }

        public Dictionary<string, float> GetCurrentSensorData()
        {
            return new Dictionary<string, float>(_currentData);
        }

        /// <summary>
        /// Gets a snapshot of the currently generated data.
        /// </summary>
        /// <returns>A dictionary containing the current sensor readings.</returns>
        public Dictionary<string, float> RecordData()
        {
            return new Dictionary<string, float>(_currentData);
        }

        /// <summary>
        /// Enables or disables min/max monitoring for a specific sensor.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor to monitor (e.g., "HA1").</param>
        /// <param name="enable">True to enable monitoring, false to disable.</param>
        public void SetSensorMonitoring(string sensorId, bool enable)
        {
            if (_sensorMonitoringEnabled.ContainsKey(sensorId))
            {
                _sensorMonitoringEnabled[sensorId] = enable;
                // If enabling, reset min/max for fresh tracking
                if (enable)
                {
                    MonitoredMinMaxValues[sensorId] = (float.MaxValue, float.MinValue);
                }
            }
            else
            {
                // Add sensor to monitoring if it's a valid key from _currentData
                if (_currentData.ContainsKey(sensorId))
                {
                    _sensorMonitoringEnabled.Add(sensorId, enable);
                    if (enable)
                    {
                        MonitoredMinMaxValues[sensorId] = (float.MaxValue, float.MinValue);
                    }
                }
                else
                {
                    // Handle cases where the sensorId might not be directly a property name
                    // For now, only monitor direct property names as keys
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}