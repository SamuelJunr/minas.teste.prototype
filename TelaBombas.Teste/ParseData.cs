using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minas.teste.prototype;

namespace TelaBombas.Teste
{
    public class ParseData
    {
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
    }
}
