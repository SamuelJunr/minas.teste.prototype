using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using minas.teste.prototype.MVVM.Repository.Context;
using minas.teste.prototype.Service;
using minas.teste.prototype.MVVM.Model.Concrete;

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

            // Adiciona um manipulador para o evento ApplicationExit
            // Isso garante que a conexão serial seja fechada ao sair da aplicação.
            Application.ApplicationExit += Application_ApplicationExit;

            var finder = new ArduinoPortFinder();

            bool arduinoEncontrado = finder.FindArduinoAndConfirmData();
            ConnectionSettingsApplication.TryAutoConnect(finder); // Adicionado o ponto e vírgula para corrigir CS1002

            // Criação do modal
            using (var infoForm = new InformationForm(arduinoEncontrado, finder.ConnectedPort))
            {
                infoForm.ShowDialog();
                // Assumindo que TelaInicial é o formulário principal que mantém a aplicação rodando
                Application.Run(new TelaInicial(finder.ConnectedPort));
            }

            // Configuração do HostBuilder para injeção de dependência
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=supervisory_SYSMT_data.db")); // Substitua pelo seu connection string
                })
                .Build();

            // O host.Run() não será alcançado se Application.Run for usado para o ciclo de vida principal da UI.
            // Se você mudar para um modelo de host para a UI, o encerramento do host
            // também pode ser um ponto para fechar a conexão serial.
        }

        /// <summary>
        /// Manipulador para o evento ApplicationExit. Chamado quando a aplicação está saindo.
        /// </summary>
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Garante que a conexão serial persistente seja fechada ao sair da aplicação.
            ConnectionSettingsApplication.CloseAllConnections();
        }

        // Adicionado o método TryAutoConnect para corrigir CS0103

    }
}
