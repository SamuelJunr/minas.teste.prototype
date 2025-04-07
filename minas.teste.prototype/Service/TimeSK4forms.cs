using System;
using System.Windows.Forms;

namespace minas.teste.prototype.Service
{
    

    public static class TimeSK4forms
        {
        /// <summary>
        /// Configura um DateTimePicker para exibir apenas a hora atual do sistema,
        /// usando um formato personalizado (padrão "HH:mm").
        /// </summary>
        /// <param name="picker">O DateTimePicker a ser configurado.</param>
        /// <param name="customFormat">O formato de hora desejado (ex: "HH:mm", "HH:mm:ss", "hh:mm tt").</param>
        public static void CurrentTime(this DateTimePicker picker, string customFormat = "HH:mm:ss")
        {
            if (picker == null) return; // Segurança

            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = customFormat;
            picker.ShowUpDown = true;
            picker.Value = DateTime.Now; // Define a hora atual
        }

        /// <summary>
        /// Configura um DateTimePicker para exibir apenas a hora atual do sistema,
        /// usando o formato de hora padrão do sistema operacional.
        /// </summary>
        /// <param name="picker">O DateTimePicker a ser configurado.</param>
        public static void TimeSystemFormat(this DateTimePicker picker)
        {
            if (picker == null) return; // Segurança

            picker.Format = DateTimePickerFormat.Time; // Usa o formato de hora do sistema
            picker.ShowUpDown = true;
            picker.Value = DateTime.Now; // Define a hora atual
        }
    }
}

