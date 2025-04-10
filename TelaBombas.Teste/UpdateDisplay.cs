using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype;

namespace TelaBombas.Teste
{
    class UpdateDisplay
    {
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
    }
}
