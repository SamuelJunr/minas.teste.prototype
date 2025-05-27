using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;
// Assuming Properties.Settings is accessible for calibration coefficients
// using minas.teste.prototype.Properties; 

namespace minas.teste.prototype.MVVM.ViewModel
{
    public class Tela_BombasVM
    {
        private cronometroSK4 _cronometro = new cronometroSK4();
        private System.Windows.Forms.Timer _timerAtualizacao;
        public const string LABEL_CRONOMETRO = "labelCronometro_bomba";
        private HoraDia _Tempo;
        private SessaoBomba _sessaoBomba;
        public event EventHandler<Datapoint_Bar_Rpm> Chart1Data; // Assuming Datapoint_Bar_Rpm is defined

        // --- Chart Data Logic Fields ---
        private double _previousRotationRpm = double.NaN;
        private const double RotationTolerance = 10.0;

        public struct Datapoint_Bar_Lpm
        {
            public double PressureBar { get; set; }
            public double FlowLpm { get; set; }

            public Datapoint_Bar_Lpm(double pressureBar, double flowLpm)
            {
                PressureBar = pressureBar;
                FlowLpm = flowLpm;
            }
        }
        public struct Datapoint_Bar_Rpm // Make sure this is defined if used by Chart1Data event
        {
            public double RotationRpm { get; set; }
            public double PressureBar { get; set; }

            public Datapoint_Bar_Rpm(double rotationRpm, double pressureBar)
            {
                RotationRpm = rotationRpm;
                PressureBar = pressureBar;
            }
        }


        // --- Serial Data Processing and Calibration Fields ---
        private Dictionary<string, double> _currentRawSensorReadings = new Dictionary<string, double>();
        private Dictionary<string, double> _currentCalibratedSensorReadings = new Dictionary<string, double>();
        private Dictionary<string, double> _calibrationFactors = new Dictionary<string, double>();

        public event Action<Dictionary<string, string>> SensorDisplayDataUpdated;

        public readonly Dictionary<string, string> _serialKeyToTextBoxNameMap = new Dictionary<string, string>
        {
            {"HA1", "sensor_HA1"}, {"HA2", "sensor_HA2"},
            {"HB1", "sensor_HB1"}, {"HB2", "sensor_HB2"},
            {"MA1", "sensor_MA1"}, {"MA2", "sensor_MA2"},
            {"MB1", "sensor_MB1"}, {"MB2", "sensor_MB2"},
            {"TEM", "sensor_CELSUS"},
            {"ROT", "sensor_RPM"},
            {"DR1", "sensor_DR1"}, {"DR2", "sensor_DR2"},
            {"PL1", "sensor_P1"}, {"PL2", "sensor_P2"}, // Maps PLx from serial to sensor_Px TextBox
            {"PL3", "sensor_P3"}, {"PL4", "sensor_P4"},
            {"PR1", "sensor_PR1"}, {"PR2", "sensor_PR2"}, // Maps PRx from serial to sensor_PRx TextBox
            {"PR3", "sensor_PR3"}, {"PR4", "sensor_PR4"},
            {"VZ1", "sensor_V1"}, {"VZ2", "sensor_V2"},
            {"VZ3", "sensor_V3"}, {"VZ4", "sensor_V4"}
            // DR3, DR4 from serial are not mapped to a specific TextBox here for direct update via event.
            // Their values will be in _currentCalibratedSensorReadings and can be fetched via GetCalibratedSensorValue.
        };

        public Tela_BombasVM()
        {
            LoadCalibrationFactors();
        }

        private void LoadCalibrationFactors()
        {
            _calibrationFactors.Clear();
            try
            {
                string json = minas.teste.prototype.Properties.Settings.Default.CalibrationCoefficientsJSON;
                if (!string.IsNullOrEmpty(json))
                {
                    var serializer = new JavaScriptSerializer();
                    var savedCoefficients = serializer.Deserialize<Dictionary<string, string>>(json);

                    if (savedCoefficients != null)
                    {
                        foreach (var entry in savedCoefficients)
                        {
                            if (double.TryParse(entry.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double factor) && factor != 0.0)
                            {
                                _calibrationFactors[entry.Key] = factor;
                            }
                            else
                            {
                                _calibrationFactors[entry.Key] = 1.0;
                                Debug.WriteLine($"Warning: Calibration factor for {entry.Key} ('{entry.Value}') is invalid or zero. Using 1.0.");
                            }
                        }
                        Debug.WriteLine("ViewModel: Calibration factors loaded.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ViewModel: Error loading calibration factors: {ex.Message}");
            }
        }

        public void ProcessSerialDataString(string dataString)
        {
            if (string.IsNullOrWhiteSpace(dataString)) return;

            string[] pairs = dataString.Split('|');
            bool newReadingsFound = false;
            var tempRawReadings = new Dictionary<string, double>(); // Process one full message

            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    if (double.TryParse(keyValue[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        tempRawReadings[key] = value; // Use temp dictionary for the current message
                        newReadingsFound = true;
                    }
                    else
                    {
                        Debug.WriteLine($"[VM_PARSE_ERROR] Invalid value for {key}: {keyValue[1]} in message: {dataString}");
                    }
                }
            }

            // Update main raw readings with the fresh set from the current message
            // This ensures that if a sensor isn't in the current message, its old value isn't stuck
            // For a full overwrite strategy:
            // _currentRawSensorReadings = new Dictionary<string, double>(tempRawReadings);
            // For an update/add strategy:
            foreach (var reading in tempRawReadings)
            {
                _currentRawSensorReadings[reading.Key] = reading.Value;
            }


            if (newReadingsFound) // Or always apply calibration even if only one value changed
            {
                ApplyCalibration();
                PrepareDisplayValuesAndNotifyView();
            }
        }

        private void ApplyCalibration()
        {
            _currentCalibratedSensorReadings.Clear();
            foreach (var rawReading in _currentRawSensorReadings)
            {
                string sensorKey = rawReading.Key;
                double rawValue = rawReading.Value;
                double calibratedValue = rawValue;

                if (_calibrationFactors.TryGetValue(sensorKey, out double factor))
                {
                    calibratedValue = rawValue * factor; // Factor of 1.0 means no change
                }
                _currentCalibratedSensorReadings[sensorKey] = calibratedValue;
            }
        }

        private void PrepareDisplayValuesAndNotifyView()
        {
            var displayValues = new Dictionary<string, string>();
            foreach (var calibratedReading in _currentCalibratedSensorReadings)
            {
                string serialKey = calibratedReading.Key;
                double value = calibratedReading.Value;
                string formattedValue;

                if (serialKey == "TEM") formattedValue = value.ToString("F1", CultureInfo.InvariantCulture);
                else if (serialKey == "ROT") formattedValue = value.ToString("F0", CultureInfo.InvariantCulture);
                else formattedValue = value.ToString("F2", CultureInfo.InvariantCulture);

                if (_serialKeyToTextBoxNameMap.TryGetValue(serialKey, out string textBoxName))
                {
                    displayValues[textBoxName] = formattedValue;
                }
            }
            SensorDisplayDataUpdated?.Invoke(displayValues);
        }

        public double GetCalibratedSensorValue(string serialKey, double defaultValue = 0.0)
        {
            if (_currentCalibratedSensorReadings.TryGetValue(serialKey, out double value))
            {
                return value;
            }
            // If key not found (e.g. DR3, DR4 if not mapped), return default
            // Or if you want raw value for uncalibrated/unmapped:
            // if (_currentRawSensorReadings.TryGetValue(serialKey, out double rawValue)) return rawValue;
            return defaultValue;
        }

        // ... (Rest of the Tela_BombasVM.cs methods like Carregar_configuracao, cronometer, validation, buttons, charts, flags, etc., remain as previously refactored)
        #region PROPRIEDADES_JANELA 
        public void Carregar_configuracao(Form FormView)
        {
            FormView.Text = minas.teste.prototype.Properties.Resources.ResourceManager.GetString("MainFormTitle");
            SetupCronometroTimer();
            VincularCronometroLabel(FormView);
        }
        public void Stage_signal(PictureBox stage)
        {
            stage.BackgroundImage = (System.Drawing.Image)minas.teste.prototype.Properties.Resources.ResourceManager.GetObject("off");
        }
        public void VincularRelogioLabel(Label relogio)
        {
            _Tempo = new HoraDia(relogio);
        }
        #endregion

        #region CRONÔMETRO 
        private void SetupCronometroTimer()
        {
            _timerAtualizacao = new System.Windows.Forms.Timer();
            _timerAtualizacao.Interval = 100;
            _timerAtualizacao.Tick += (s, e) => AtualizarDisplayCronometro();
        }

        private void VincularCronometroLabel(Form view)
        {
            _cronometro.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_cronometro.FormattedElapsed))
                {
                    var label = view.Controls.Find(LABEL_CRONOMETRO, true).FirstOrDefault() as Label;
                    AtualizarLabelSeguro(label, _cronometro.FormattedElapsed);
                }
            };
        }

        public void IniciarCronometro()
        {
            if (_cronometro.IsRunning) return;
            if (_cronometro.GetCurrentTotalElapsedTime() > TimeSpan.Zero)
                _cronometro.AccumulatedElapsed = TimeSpan.Zero;
            _cronometro.StartTime = DateTime.Now;
            _cronometro.IsRunning = true;
            _timerAtualizacao.Start();
        }

        public void PararCronometro()
        {
            if (!_cronometro.IsRunning) return;
            _cronometro.AccumulatedElapsed = _cronometro.GetCurrentTotalElapsedTime();
            _cronometro.IsRunning = false;
            _timerAtualizacao.Stop();
            AtualizarDisplayCronometro();
        }

        private void AtualizarDisplayCronometro()
        {
            _cronometro.UpdateFormattedString();
        }

        private void AtualizarLabelSeguro(Label label, string valor)
        {
            if (label == null || label.IsDisposed) return;
            if (label.InvokeRequired)
                label.BeginInvoke((Action)(() => { if (!label.IsDisposed) label.Text = valor; }));
            else
                label.Text = valor;
        }
        #endregion

        #region VALIDACAO 
        public void MonitorarDados(
           string psiPLValor, string psiPLMin, string psiPLMax, bool psiPLAtivo, Panel psiPLPanel, TextBox historicalEvents,
           string pressPSIValor, string pressPSIMin, string pressPSIMax, bool pressPSIAtivo, Panel pressPSIPanel,
           string gpmDRValor, string gpmDRMin, string gpmDRMax, bool gpmDRAtivo, Panel gpmDRPanel,
           string vazaoGPMValor, string vazaoGPMMin, string vazaoGPMMax, bool vazaoGPMAtivo, Panel vazaoGPMPanel,
           string pressBARValor, string pressBARMin, string pressBARMax, bool pressBARAtivo, Panel pressBARPanel,
           string barPLValor, string barPLMin, string barPLMax, bool barPLAtivo, Panel barPLPanel,
           string rotacaoRPMValor, string rotacaoRPMMin, string rotacaoRPMMax, bool rotacaoRPMAtivo, Panel rotacaoRPMPanel,
           string vazaoLPMValor, string vazaoLPMMin, string vazaoLPMMax, bool vazaoLPMAtivo, Panel vazaoLPMPanel,
           string lpmDRValor, string lpmDRMin, string lpmDRMax, bool lpmDRAtivo, Panel lpmDRPanel,
           string tempCValor, string tempCMin, string tempCMax, bool tempCAtivo, Panel tempCPanel
       )
        {
            VerificarRange("Pilotagem PSI", psiPLValor, psiPLMin, psiPLMax, psiPLAtivo, psiPLPanel, historicalEvents);
            VerificarRange("Pressão PSI", pressPSIValor, pressPSIMin, pressPSIMax, pressPSIAtivo, pressPSIPanel, historicalEvents);
            VerificarRange("Dreno GPM", gpmDRValor, gpmDRMin, gpmDRMax, gpmDRAtivo, gpmDRPanel, historicalEvents);
            VerificarRange("Vazão GPM", vazaoGPMValor, vazaoGPMMin, vazaoGPMMax, vazaoGPMAtivo, vazaoGPMPanel, historicalEvents);
            VerificarRange("Pressão BAR", pressBARValor, pressBARMin, pressBARMax, pressBARAtivo, pressBARPanel, historicalEvents);
            VerificarRange("Pilotagem BAR", barPLValor, barPLMin, barPLMax, barPLAtivo, barPLPanel, historicalEvents);
            VerificarRange("Rotação RPM", rotacaoRPMValor, rotacaoRPMMin, rotacaoRPMMax, rotacaoRPMAtivo, rotacaoRPMPanel, historicalEvents);
            VerificarRange("Vazão LPM", vazaoLPMValor, vazaoLPMMin, vazaoLPMMax, vazaoLPMAtivo, vazaoLPMPanel, historicalEvents);
            VerificarRange("Dreno LPM", lpmDRValor, lpmDRMin, lpmDRMax, lpmDRAtivo, lpmDRPanel, historicalEvents);
            VerificarRange("Temperatura Celsus", tempCValor, tempCMin, tempCMax, tempCAtivo, tempCPanel, historicalEvents);
        }

        private void SetPanelImage(Panel panel, string resourceName)
        {
            if (panel == null || panel.IsDisposed) return;

            Action action = () =>
            {
                if (string.IsNullOrEmpty(resourceName))
                {
                    panel.BackgroundImage?.Dispose();
                    panel.BackgroundImage = null;
                    panel.BackColor = SystemColors.Control;
                    return;
                }
                try
                {
                    object resourceObject = minas.teste.prototype.Properties.Resources.ResourceManager.GetObject(resourceName);
                    if (resourceObject is byte[] imageBytes) { using (var ms = new MemoryStream(imageBytes)) { panel.BackgroundImage?.Dispose(); panel.BackgroundImage = System.Drawing.Image.FromStream(ms); } }
                    else if (resourceObject is System.Drawing.Image image) { panel.BackgroundImage?.Dispose(); panel.BackgroundImage = (System.Drawing.Image)image.Clone(); }
                    else { panel.BackgroundImage?.Dispose(); panel.BackgroundImage = null; panel.BackColor = Color.Magenta; Debug.WriteLine($"Alerta: Recurso '{resourceName}' não encontrado ou não é uma imagem válida."); }
                }
                catch (Exception ex) { panel.BackgroundImage?.Dispose(); panel.BackgroundImage = null; panel.BackColor = Color.Red; Debug.WriteLine($"Erro ao definir imagem do painel com recurso '{resourceName}': {ex.Message}"); }
            };

            if (panel.InvokeRequired) panel.BeginInvoke(action); else action();
        }

        private void VerificarRange(string sensorNome, string sensorValorTexto, string minValorTexto, string maxValorTexto, bool ativo, Panel panelAlerta, TextBox historicalEvents)
        {
            if (panelAlerta == null || panelAlerta.IsDisposed) return;
            string resourceNameOn = null; string resourceNameOff = null;
            switch (sensorNome)
            {
                case "Pilotagem PSI": resourceNameOn = "pilotagem_on"; resourceNameOff = "pilotagem_off"; break;
                case "Pressão PSI": resourceNameOn = "pressao_on"; resourceNameOff = "pressao_off"; break;
                // ... (rest of the cases) ...
                case "Temperatura Celsus": resourceNameOn = "termometro_on"; resourceNameOff = "termometro_off"; break;
                default: Debug.WriteLine($"Aviso: Nome de sensor não mapeado para imagens: '{sensorNome}'"); SetPanelImage(panelAlerta, null); panelAlerta.BackColor = Color.Gray; return;
            }

            if (!ativo) { SetPanelImage(panelAlerta, resourceNameOn); return; }
            bool valorOk = decimal.TryParse(sensorValorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorSensor);
            bool minOk = decimal.TryParse(minValorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxValorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);

            if (valorOk && minOk && maxOk)
            {
                if (valorSensor < valorMinimo || valorSensor > valorMaximo)
                {
                    SetPanelImage(panelAlerta, resourceNameOff);
                    if (historicalEvents != null && !historicalEvents.IsDisposed)
                    {
                        Action appendLog = () => historicalEvents.AppendText($"{DateTime.Now:G}: O valor do sensor {sensorNome} ({valorSensor}) está fora do range [{valorMinimo} - {valorMaximo}]." + Environment.NewLine);
                        if (historicalEvents.InvokeRequired) historicalEvents.BeginInvoke(appendLog); else appendLog();
                    }
                }
                else { SetPanelImage(panelAlerta, resourceNameOn); }
            }
            else { SetPanelImage(panelAlerta, resourceNameOff); }
        }
        #endregion

        #region BOTÕES 
        public void IniciarTesteBomba(PictureBox stage)
        {
            IniciarCronometro();
            if (stage != null && !stage.IsDisposed)
            {
                var img = minas.teste.prototype.Properties.Resources.ResourceManager.GetObject("on") as System.Drawing.Image;
                if (img != null) stage.BackgroundImage = img;
            }
            _sessaoBomba = new SessaoBomba();
        }

        public void FinalizarTesteBomba(PictureBox stage)
        {
            PararCronometro();
            if (stage != null && !stage.IsDisposed)
            {
                var img = minas.teste.prototype.Properties.Resources.ResourceManager.GetObject("off") as System.Drawing.Image;
                if (img != null) stage.BackgroundImage = img;
            }
            if (_sessaoBomba != null) { _sessaoBomba.FinalizarSessao(); mensagemConfirmacao(); }
        }

        public void mensagemConfirmacao()
        {
            var resultado = MessageBox.Show("Deseja salvar todos os dados do teste?", "Confirmação de Salvamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (resultado == DialogResult.Yes) { _sessaoBomba.ProcessarSalvamento(); }
            else { MessageBox.Show("Dados não foram salvos!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        public bool cabecalhoinicial(TextBox textBox6, TextBox textBox5, TextBox textBox4)
        {
            return !(string.IsNullOrEmpty(textBox6?.Text) || string.IsNullOrEmpty(textBox5?.Text) || string.IsNullOrEmpty(textBox4?.Text));
        }

        public void PiscarLabelsVermelhoSync(Label label1, Label label2, Label label3, int duration)
        {
            if (label1 == null || label2 == null || label3 == null) return;
            Color originalColor1 = label1.ForeColor; Color originalColor2 = label2.ForeColor; Color originalColor3 = label3.ForeColor;
            int interval = 500; int steps = duration / interval; int totalTicks = 2 * steps;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = interval }; int tickCount = 0;
            timer.Tick += (sender, e) => {
                if (label1.IsDisposed || label2.IsDisposed || label3.IsDisposed) { timer.Stop(); timer.Dispose(); return; }
                label1.ForeColor = (tickCount % 2 == 0) ? Color.Red : originalColor1;
                label2.ForeColor = (tickCount % 2 == 0) ? Color.Red : originalColor2;
                label3.ForeColor = (tickCount % 2 == 0) ? Color.Red : originalColor3;
                tickCount++;
                if (tickCount >= totalTicks) { timer.Stop(); if (!label1.IsDisposed) label1.ForeColor = originalColor1; if (!label2.IsDisposed) label2.ForeColor = originalColor2; if (!label3.IsDisposed) label3.ForeColor = originalColor3; timer.Dispose(); }
            };
            timer.Start();
        }

        public void LimparCamposEntrada(params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes) { if (textBox != null && !textBox.IsDisposed) textBox.Text = string.Empty; }
        }
        #endregion

        #region GRAFICOS 
        public Datapoint_Bar_Lpm? GetChartDataIfRotationConstant(string pressureBarText, string vazaoLpmText, string rotacaoRpmText)
        {
            if (double.TryParse(pressureBarText, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressureBar) &&
                double.TryParse(vazaoLpmText, NumberStyles.Any, CultureInfo.InvariantCulture, out double flowLpm) &&
                double.TryParse(rotacaoRpmText, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentRotationRpm))
            {
                if (double.IsNaN(_previousRotationRpm) || Math.Abs(currentRotationRpm - _previousRotationRpm) <= RotationTolerance)
                { _previousRotationRpm = currentRotationRpm; return new Datapoint_Bar_Lpm(pressureBar, flowLpm); }
                else { _previousRotationRpm = currentRotationRpm; return null; }
            }
            _previousRotationRpm = double.NaN; return null;
        }

        public void ResetChartDataLogic() { _previousRotationRpm = double.NaN; }
        public void ProcessChartData(double rotation, double pressure) { Chart1Data?.Invoke(this, new Datapoint_Bar_Rpm(rotation, pressure)); }
        #endregion

        #region FLAGS 
        public void AlterarEstadoPaineis(bool ativo, Panel p1, Panel p2, Panel p3, Panel p4, Panel p5, Panel p6)
        {
            string suffix = ativo ? "_on" : "_by";
            SetPanelImage(p1, $"pilotagem{suffix}");
            SetPanelImage(p2, $"dreno{suffix}");
            SetPanelImage(p3, $"pressao{suffix}");
            SetPanelImage(p4, $"rotacao{suffix}");
            SetPanelImage(p5, $"vazao{suffix}");
            SetPanelImage(p6, $"termometro{suffix}");
        }
        #endregion

        public void AtualizarVisualizador(DataGridView fonte, List<SensorData> data) // Assuming SensorData is defined
        {
            if (fonte == null || fonte.IsDisposed) return;
            Action updateGrid = () =>
            {
                if (fonte.Columns.Count == 0)
                {
                    fonte.AutoGenerateColumns = false;
                    fonte.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Sensor", HeaderText = "Sensor", Width = 100 });
                    fonte.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Valor", HeaderText = "Valor", Width = 80 });
                    fonte.Columns.Add(new DataGridViewTextBoxColumn() { DataPropertyName = "Medidas", HeaderText = "Medidas", Width = 60 });
                }
                fonte.DataSource = null;
                fonte.DataSource = data;
            };
            if (fonte.InvokeRequired) fonte.BeginInvoke(updateGrid); else updateGrid();
        }
        public static void TiposdeTesteBombas() { /* ... */ }
    }
}