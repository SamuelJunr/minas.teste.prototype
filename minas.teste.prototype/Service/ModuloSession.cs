using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace minas.teste.prototype.Service
{
    public class ModuleSession
    {
        private readonly Stopwatch moduleTimer = new Stopwatch();
        private string ModuleName { get; set; }
        public TimeSpan ExecutionTime => moduleTimer.Elapsed;

        public void Session_ini(bool ardstage, string moduleName)
        {
            if (ardstage)
            {
                ModuleSession_ini(moduleName);
            }
        }

        public void ModuleSession_ini(string moduleName)
        {
            ModuleName = moduleName;
            moduleTimer.Start();
        }

        public void EndModule()
        {
            moduleTimer.Stop();
            SaveModuleSession();
        }

        private void SaveModuleSession()
        {
            // Adapte para seu método de persistência
            var moduleData = new
            {
                ModuleName,
                ExecutionTime.TotalSeconds,
                Timestamp = DateTime.Now
            };

            File.WriteAllText($"module_{ModuleName}_log.json",
                              JsonConvert.SerializeObject(moduleData));
        }
    }
}
