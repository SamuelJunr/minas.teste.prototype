using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using minas.teste.prototype.MVVM.Repository.Context; // Seu AppDbContext
using minas.teste.prototype.Service;                 // Onde ArduinoPortFinder deve estar
using minas.teste.prototype.MVVM.Model.Concrete;     // Onde ConnectionSettingsApplication está
using minas.teste.prototype.MVVM.View;               // Para InformationForm, TelaInicial
using System.Diagnostics;

namespace minas.teste.prototype
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += Application_ApplicationExit;

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=supervisory_SYSMT_data.db")); // Substitua pela sua connection string
                })
                .Build();

            ServiceProvider = host.Services;
            Debug.WriteLine("Program: Host construído e ServiceProvider configurado.");

            // 1. Tentativa de Autoconexão
            // Instancia o ArduinoPortFinder do namespace correto (Service)
            // Se CS0246 ocorre aqui, é porque a classe ArduinoPortFinder
            // não está definida corretamente no namespace minas.teste.prototype.Service
            var finder = new ArduinoPortFinder();
            bool autoConnectSuccess = false;

            Debug.WriteLine("Program: Tentando autoconexão...");

            // A chamada para ConnectionSettingsApplication.TryAutoConnect DEVE esperar
            // um minas.teste.prototype.Service.ArduinoPortFinder como argumento.
            if (ConnectionSettingsApplication.TryAutoConnect(finder: finder))
            {
                autoConnectSuccess = true;
                Debug.WriteLine($"Program: Autoconexão bem-sucedida na porta {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate} bps.");
            }
            else
            {
                Debug.WriteLine("Program: Autoconexão falhou ou nenhuma porta Arduino válida foi encontrada/confirmada.");
            }

            // 2. Exibir formulário de informação sobre a autoconexão
            // Passa o resultado e a porta (se conectada via ConnectionSettingsApplication)
            using (var infoForm = new InformationForm(autoConnectSuccess, ConnectionSettingsApplication.CurrentPortName))
            {
                Debug.WriteLine("Program: Exibindo InformationForm.");
                infoForm.ShowDialog();
            }
            Debug.WriteLine("Program: InformationForm fechado.");

            // 3. Iniciar o Formulário Principal
            Debug.WriteLine("Program: Iniciando TelaInicial.");
            // TelaInicial pode verificar ConnectionSettingsApplication.IsCurrentlyConnected
            // e ConnectionSettingsApplication.CurrentPortName para saber o estado da conexão.
            Application.Run(new TelaInicial());
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Debug.WriteLine("Program: ApplicationExit disparado. Fechando todas as conexões seriais.");
            ConnectionSettingsApplication.CloseAllConnections();
            Debug.WriteLine("Program: Conexões seriais fechadas.");
        }
    }
}
