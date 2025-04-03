using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Service;

namespace minas.teste.prototype
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var finder = new ArduinoPortFinder();
            bool arduinoEncontrado = finder.FindArduino();

            // Criação do modal
            using (var infoForm = new InformationForm(arduinoEncontrado, finder.ConnectedPort))
            {
                infoForm.ShowDialog();
                Application.Run(new TelaInicial(finder.ConnectedPort));
            }

           
                
            
        }


    }
}

