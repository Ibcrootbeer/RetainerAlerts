using System.Numerics;

using Dalamud.Interface.Windowing;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using Dalamud.Bindings.ImGui;

namespace RetainerAlerts.Windows
{
    internal unsafe class AlertWindow : Window
    {
        private Plugin plugin;
        private Vector4 defaultBackgroundColor = new Vector4(.25f, .89f, .96f, 0.3f);
        private Vector4 repositionBackgroundColor = new Vector4(.87f, .13f, .13f, 0.8f);
        private string defaultAlertText = "Venture Completed";
        private string repositionAlertText = "Reposition Me!";
        private string dataAlertText = "Click Me Twice";

        private Vector4 backgroundColor = new Vector4(.25f, .89f, .96f, 0.3f);
        private string alertText = "Venture Completed";

        public AlertWindow(Plugin plugin) : base("Alert Window##RetainerAlerts")
        {
            this.plugin = plugin;
            BgAlpha = 0;
            Flags =
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollWithMouse |
                ImGuiWindowFlags.AlwaysAutoResize |
                ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoNavFocus;
            AllowClickthrough = true;
            RespectCloseHotkey = false;
        }

        public override void PreDraw()
        {
            if (plugin.Configuration.IsAlertMovable)
            {
                Flags &= ~ImGuiWindowFlags.NoMove;
                backgroundColor = repositionBackgroundColor;
                alertText = repositionAlertText;
            }
            else
            {
                Flags |= ImGuiWindowFlags.NoMove;
                backgroundColor = defaultBackgroundColor;
                alertText = defaultAlertText;
            }

            if (plugin.shouldShowTimersText)
            {
                backgroundColor = repositionBackgroundColor;
                alertText = dataAlertText;
            }
        }

        public override void Draw()
        {
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1f, 0f, 0f, 1f));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, backgroundColor);
            ImGui.Begin("ActualAlertWindow", this.Flags);
            ImGui.Text(alertText);
            if (plugin.shouldShowTimersText && ImGui.IsWindowHovered() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                AgentActionMenu.Instance()->UIModuleInterface->ExecuteMainCommand(5);
            }
            ImGui.End();
            ImGui.PopStyleColor(2);
        }
    }
}
