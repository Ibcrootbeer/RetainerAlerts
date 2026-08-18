using System;
using System.Numerics;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace RetainerAlerts.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Plugin plugin;
    private Configuration configuration;
    private int alertWindowCondition;
    private float ColorR = 0f;
    private float ColorG = 0f;
    private float ColorB = 0f;
    private float ColorT = 0f;

    public ConfigWindow(Plugin plugin, Vector4 AlertWindowColor) : base("Retainer Alerts Configuration###RetainerAlerts")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        ColorR = AlertWindowColor.X;
        ColorG = AlertWindowColor.Y;
        ColorB = AlertWindowColor.Z;
        ColorT = AlertWindowColor.W;

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize;

        this.plugin = plugin;
        this.configuration = plugin.Configuration;
        this.alertWindowCondition = configuration.AlertWindowCondition;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.LabelText(string.Empty, "Reposition/Resize");
        if (ImGui.Button("Reposition Popup"))
        {
            plugin.ToggleAlertMovement();
        }

        ImGui.LabelText(string.Empty, "Show Alert When:");
        if (ImGui.Combo(string.Empty, ref alertWindowCondition, AlertWindowCondition.Conditions, AlertWindowCondition.Conditions.Length))
        {
            plugin.ChangeAlertCondition(alertWindowCondition);
        }

        ImGui.LabelText(string.Empty, "Alert Window Color");

        if (ImGui.SliderFloat("Red", ref ColorR, 0f, 1f))
        {
            plugin.SetBackgroundColor(new Vector4(ColorR, ColorG, ColorB, ColorT));
        }

        if (ImGui.SliderFloat("Green", ref ColorG, 0f, 1f))
        {
            plugin.SetBackgroundColor(new Vector4(ColorR, ColorG, ColorB, ColorT));
        }

        if (ImGui.SliderFloat("Blue", ref ColorB, 0f, 1f))
        {
            plugin.SetBackgroundColor(new Vector4(ColorR, ColorG, ColorB, ColorT));
        }

        if (ImGui.SliderFloat("Alpha", ref ColorT, 0f, 1f))
        {
            plugin.SetBackgroundColor(new Vector4(ColorR, ColorG, ColorB, ColorT));
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        plugin.SetAlertWindowStatus();
    }
}
