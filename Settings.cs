internal sealed class Settings : ApplicationSettingsBase, INotifyPropertyChanged
{
    private static Settings defaultInstance;
    public static Settings Default { get; }

    public string TipoSensorSelecionado { get; set; }
    public string PortaCOMSelecionada { get; set; }
      
    public int BaudRateSelecionado { get; set; }

    private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e);
    private void SettingsSavingEventHandler(object sender, CancelEventArgs e);
}
