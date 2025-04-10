using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minas.teste.prototype;
using Moq;

namespace TelaBombas.Teste
{
    class SerialComunication
    {
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
    }
}
