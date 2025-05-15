using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using minas.teste.prototype.MVVM.Repository.Context;
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

            // Configuração do HostBuilder para injeção de dependência
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=supervisory_SYSMT_data.db")); // Substitua pelo seu connection string
                })
                .Build();

            // Opcional: Inicializar o banco de dados
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}
