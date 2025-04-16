using System;
using System.Media;
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

    private const string CommandName = "/retaineralerts";
    private const string CommandNameAlias = "/ra";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("RetainerAlerts");
    private ConfigWindow ConfigWindow { get; init; }
    private AlertWindow AlertWindow { get; init; }

    // Potentially configurable?
    private static System.Timers.Timer Timer = new System.Timers.Timer(5000);

    public bool shouldShowTimersText = true;

    public bool testThingy = true;

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

        Timer.Elapsed += checkTimes;
        Timer.AutoReset = true;
        Timer.Enabled = true;

        SetAlertWindowStatus();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        Timer.Dispose();

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
        Configuration.Save();
        SetAlertWindowStatus();
    }

    private void checkTimes(Object? source, ElapsedEventArgs e)
    {
        if (Retainer.UpdateRetainers())
        {
            shouldShowTimersText = false;
        }
        SetAlertWindowStatus();
    }

    public void ToggleAlertMethod()
    {
        SetAlertWindowStatus();
    }

    public void ChangeAlertMethod(int whenToshowAlert)
    {
        Configuration.WhenToShowAlert = whenToshowAlert;
        Configuration.Save();
        SetAlertWindowStatus();
    }

    private void SetAlertWindowStatus()
    {
        bool ventureCheck;

        // TODO The cases are currently tied to the order in the ImGui.Combo element. Change this to an enum.
        switch (Configuration.WhenToShowAlert)
        {
            case 0:
                ventureCheck = Retainer.AnyVenturesComplete();
                break;
            case 1:
                ventureCheck = Retainer.AreAllVenturesComplete();
                break;
            default:
                ventureCheck = Retainer.AnyVenturesComplete();
                break;
        }  

        // TODO Add in stuff for hiding it when in cutscene?
        if (ClientState.IsLoggedIn)
        {
            AlertWindow.IsOpen = (ventureCheck || Configuration.IsAlertMovable || shouldShowTimersText);
        }
        else
        {
            AlertWindow.IsOpen = false;
        }
    }
}
