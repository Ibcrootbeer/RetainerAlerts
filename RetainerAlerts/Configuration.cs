using System;

using Dalamud.Configuration;

namespace RetainerAlerts;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool IsConfigWindowMovable { get; set; } = true;
    public bool IsAlertMovable { get; set; } = false;
    public int AlertWindowCondition { get; set; } = 0;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
