using System.Numerics;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace RetainerAlerts.Windows
{
    internal unsafe class AlertWindow : Window
    {
        private Plugin plugin;
        private Vector4 backGroundColor = Vector4.Zero;
        private Vector4 defaultBackGroundColor = new Vector4(.25f, .89f, .96f, 0.3f);
        public Vector4 CustomBackgroundColor = Vector4.Zero;
        private Vector4 repositionBackgroundColor = new Vector4(.87f, .13f, .13f, 0.8f);
        private string defaultAlertText = "Venture Completed";
        private string repositionAlertText = "Reposition Me!";
        private string dataAlertText = "Click Me Twice";
        private string alertText = "Venture Completed";

        public AlertWindow(Plugin plugin, Vector4 CustomAlertWindowColor) : base("Alert Window##RetainerAlerts")
        {
            this.plugin = plugin;
            Flags =
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollWithMouse |
                ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoNavFocus;
            AllowClickthrough = true;
            RespectCloseHotkey = false;

            if (CustomAlertWindowColor != Vector4.Zero)
            {
                CustomBackgroundColor = CustomAlertWindowColor;
                backGroundColor = CustomBackgroundColor;
            }
            else
            {
                CustomBackgroundColor = defaultBackGroundColor;
            }
        }

        public override void PreDraw()
        {
            base.PreDraw();

            if (plugin.Configuration.IsAlertMovable)
            {
                Flags &= ~ImGuiWindowFlags.NoMove;
                Flags &= ~ImGuiWindowFlags.NoResize;
                backGroundColor = repositionBackgroundColor;
                alertText = repositionAlertText;
            }
            else
            {
                Flags |= ImGuiWindowFlags.NoMove;
                Flags |= ImGuiWindowFlags.NoResize;
                backGroundColor = CustomBackgroundColor;
                alertText = defaultAlertText;
            }

            if (plugin.shouldShowTimersText)
            {
                backGroundColor = repositionBackgroundColor;
                alertText = dataAlertText;
            }

            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1f, 0f, 0f, 1f));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, backGroundColor);
        }

        public override void PostDraw()
        {
            base.PostDraw();

            ImGui.PopStyleColor(2);
        }

        // TODO I hate the hard coded reductions, but it looks so much better.
        public override void Draw()
        {
            ImGui.AlignTextToFramePadding();
            float windowWidth = ImGui.GetContentRegionAvail().X;
            float textWidth = ImGui.CalcTextSize(alertText).X;
            textWidth -= 15;
            float xpos = (windowWidth - textWidth) * 0.5f;
            ImGui.SetCursorPosX(xpos);

            float windowHeight = ImGui.GetContentRegionAvail().Y;
            float textHeight = ImGui.CalcTextSize(alertText).Y;
            textHeight -= 5;
            float ypos = (windowHeight - textHeight) * 0.5f;
            ImGui.SetCursorPosY(ypos);

            ImGui.Text(alertText);
            if (plugin.shouldShowTimersText && ImGui.IsWindowHovered() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                AgentActionMenu.Instance()->UIModuleInterface->ExecuteMainCommand(5);
            }
        }
    }
}
