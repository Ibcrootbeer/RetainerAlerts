using System;
using System.Threading;
using System.Timers;

using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using RetainerAlerts.Windows;

namespace RetainerAlerts;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    private const string CommandName = "/retaineralerts";
    private const string CommandNameAlias = "/ra";
    private DateTime? lastUpdated;

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("RetainerAlerts");
    private ConfigWindow ConfigWindow { get; init; }
    private AlertWindow AlertWindow { get; init; }

    public bool shouldShowTimersText = true;

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        AlertWindow = new AlertWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(AlertWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnConfigCommand)
        {
            HelpMessage = "Opens the settings for retainer alerts."
        });
        CommandManager.AddHandler(CommandNameAlias, new CommandInfo(OnConfigCommand)
        {
            HelpMessage = "Opens the settings for retainer alerts."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        Framework.Update += UpdateAlertWindowStatus;

        SetAlertWindowStatus();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        CommandManager.RemoveHandler(CommandNameAlias);
    }

    private void OnConfigCommand(string command, string args)
    {
        ToggleConfigUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleAlertUI() => AlertWindow.Toggle();

    public void ToggleAlertMovement()
    {
        this.Configuration.IsAlertMovable = !this.Configuration.IsAlertMovable;
        this.Configuration.Save();
        SetAlertWindowStatus();
    }

    private void UpdateAlertWindowStatus(IFramework framework)
    {
        framework.Run(() =>
        {
            // TODO Potentially configurable refresh time?
            if (lastUpdated is null || lastUpdated < DateTime.Now.AddSeconds(-5))
            {
                lastUpdated = DateTime.Now;
                if (RetainerTracker.AreAnyRetainersLoaded())
                {
                    shouldShowTimersText = false;
                }
                SetAlertWindowStatus();
            }
        });
    }

    public void ChangeAlertCondition(int alertWindowCondition)
    {
        this.Configuration.AlertWindowCondition = alertWindowCondition;
        this.Configuration.Save();
        SetAlertWindowStatus();
    }

    private void SetAlertWindowStatus()
    {
        bool ventureCheck;

        // Maps to AlertWindowCondition
        switch (this.Configuration.AlertWindowCondition)
        {
            case 0:
                ventureCheck = RetainerTracker.AnyVenturesComplete();
                break;
            case 1:
                ventureCheck = RetainerTracker.AreAllVenturesComplete();
                break;
            default:
                ventureCheck = RetainerTracker.AnyVenturesComplete();
                break;
        }

        // TODO Add in stuff for hiding it when in cutscene?
        // TODO Add in stuff for hiding when on a non home-world.
        if (ClientState.IsLoggedIn)
        {
            AlertWindow.IsOpen = (ventureCheck || this.Configuration.IsAlertMovable || shouldShowTimersText);
        }
        else
        {
            AlertWindow.IsOpen = false;
        }
    }
}
