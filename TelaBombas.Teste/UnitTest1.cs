using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using Moq;
using minas.teste.prototype;
using minas.teste.prototype.MVVM.Model.Concrete;
using System.Collections.Generic;

namespace TelaBombas.Teste
{
    [TestClass]
    public class UnitTest1
    {

        var privateObject = new PrivateObject(TelaBombas);
        var currentValues = (Dictionary<string, double>)privateObject.GetField("_currentValues");

        [TestMethod]
        public void ParseData_ValidInput_UpdatesCurrentValues()
        {
            // Arrange
            var telaBombas = new Tela_Bombas();
            string rawData = "A12.5|B20.3|C30";

            // Act
            telaBombas.ParseData(rawData);

            // Assert
            Assert.AreEqual(12.5, telaBombas.GetCurrentValue("A")); // Use um método helper interno para acessar _currentValues
            Assert.AreEqual(20.3, telaBombas.GetCurrentValue("B"));
        }
        [TestMethod]
        public void UpdateDisplay_ValidData_UpdatesChartAndGrid()
        {
            // Arrange
            var telaBombas = new Tela_Bombas();
            telaBombas._vazao = new Vazao_bomba(100.5);
            telaBombas._pressao = new Pressao_bomba(5.2);

            // Act
            telaBombas.UpdateDisplay();

            // Assert
            Assert.AreEqual(1, telaBombas.chart1.Series[0].Points.Count);
            Assert.AreEqual("100.50", telaBombas.dataGridView1.Rows[0].Cells[1].Value);
        }
        [TestMethod]
        public void BtnIniciar_Click_StartsMonitoring()
        {
            // Arrange
            var telaBombas = new Tela_Bombas();
            var mockSerialManager = new Mock<PersistentSerialManager>();
            ConnectionSettingsApplication.PersistentSerialManager = mockSerialManager.Object;

            // Act
            telaBombas.btnIniciar_Click(null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(telaBombas._isMonitoring);
            mockSerialManager.Verify(m => m.DataReceived += It.IsAny<EventHandler<string>>(), Times.Once);
        }
        [TestMethod]
        public void HandleSerialData_UpdatesUIThreadSafe()
        {
            // Arrange
            var telaBombas = new Tela_Bombas();
            string testData = "G150|H200";

            // Act (executar em contexto de UI)
            AsyncContext.Run(() =>
            {
                telaBombas.HandleSerialData(null, testData);
            });

            // Assert
            Assert.AreEqual(150, telaBombas._vazao.Lpm);
            Assert.AreEqual(200, telaBombas._vazao.LpmSecundario);
        }
        [TestMethod]
        public void IniciarCronometro_StartsTimer()
        {
            // Arrange
            var telaBombas = new Tela_Bombas();

            // Act
            telaBombas.IniciarCronometro();

            // Assert
            Assert.IsTrue(telaBombas._cronometro.IsRunning);
            Assert.IsTrue(telaBombas._timerAtualizacao.Enabled);
        }
    }
}
