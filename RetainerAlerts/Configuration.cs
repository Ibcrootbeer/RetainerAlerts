using System;

using Dalamud.Configuration;

namespace RetainerAlerts;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool IsConfigWindowMovable { get; set; } = true;
    public bool IsAlertMovable { get; set; } = false;

    // If true the alert will show when any ventures are compelete, if false only when all ventures are complete.
    public bool ShouldUseAnyMethod = true;

    public int WhenToShowAlert { get; set; } = 0;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
