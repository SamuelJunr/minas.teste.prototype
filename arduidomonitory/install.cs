using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace arduidomonitory
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.ServiceName = "ArduidoMonitoryService";
            serviceInstaller.DisplayName = "Arduino Monitoring Service";
            serviceInstaller.Description = "Provides continuous monitoring of Arduino sensor data";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}