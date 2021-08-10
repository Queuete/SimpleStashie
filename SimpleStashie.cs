using ExileCore;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ImGuiNET;
using SharpDX;
using System;
using System.Collections;
using System.Windows.Forms;

namespace SimpleStashie
{
    public class SimpleStashie : BaseSettingsPlugin<SimpleStashieSettings>
    {
        private Random Random { get; } = new Random();
        private Vector2 ClickWindowOffset => GameController.Window.GetWindowRectangle().TopLeft;

        private static bool IsRunning { get; set; } = false;


        public override bool Initialise()
        {
            Input.RegisterKey(Keys.LControlKey);
            return true;
        }

        public override void Render()
        {
            if (!IsRunConditionMet()) return;
            IsRunning = true;

            var coroutineWorker = new Coroutine(StashItems(), this, "SimpleStashie.StashItems");
            Core.ParallelRunner.Run(coroutineWorker);
        }

        private bool IsRunConditionMet()
        {
            if (IsRunning) return false;
            if (!Input.GetKeyState(Settings.StashItKey.Value)) return false;
            if (!GameController.Window.IsForeground()) return false;
            if (!GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible) return false;

            if (GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal) return true;
            if (GameController.Game.IngameState.IngameUi.SellWindow.IsVisibleLocal) return true;
            if (GameController.Game.IngameState.IngameUi.TradeWindow.IsVisibleLocal) return true;

            return false;
        }

        private IEnumerator StashItems()
        {
            var items = GameController.Game.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory]?.VisibleInventoryItems;
            if (items == null)
            {
                DebugWindow.LogError("SimpleStashie -> Items in inventory is null.");
                yield break;
            }

            try
            {
                Input.KeyDown(Keys.LControlKey);
                if (Settings.IgnoreAffinities)
                {
                    Input.KeyDown(Keys.LShiftKey);
                }
                foreach (var item in items)
                {
                    if (SlotIsIgnored(item.InventPosX, item.InventPosY)) continue;
                    var centerOfItem = item.GetClientRect().Center
                        + ClickWindowOffset
                        + new Vector2(Random.Next(0, 5), Random.Next(0, 5));

                    Input.SetCursorPos(centerOfItem);
                    yield return new WaitTime(3);
                    Input.Click(MouseButtons.Left);
                    yield return new WaitTime(3);
                    Input.Click(MouseButtons.Left);

                    var waitTime = Math.Max(3, Settings.ExtraDelayInMs - 6 - 8 + Random.Next(0, 16));
                    yield return new WaitTime(waitTime);
                }
            }
            finally
            {
                Input.KeyUp(Keys.LControlKey);
                if (Settings.IgnoreAffinities)
                {
                    Input.KeyUp(Keys.LShiftKey);
                }
                IsRunning = false;
            }
        }

        private bool SlotIsIgnored(int x, int y)
        {
            if (y >= 5 || x >= 12) return true;

            return Settings.IgnoredCells[y, x] == 1;
        }

        public override void DrawSettings()
        {
            DrawIgnoredCellsSettings();
            base.DrawSettings();
        }

        private void DrawIgnoredCellsSettings()
        {
            try
            {
                ImGui.SameLine();
                ImGui.TextDisabled("(?)");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(
                        $"Checked = Item will be ignored{Environment.NewLine}UnChecked = Item will be processed");
            }
            catch (Exception e)
            {
                LogError(e.ToString(), 10);
            }

            var counter = 1;
            for (var row = 0; row < 5; row++)
            {
                for (var column = 0; column < 12; column++)
                {
                    var toggled = Convert.ToBoolean(Settings.IgnoredCells[row, column]);
                    if (ImGui.Checkbox($"##{counter}IgnoredCells", ref toggled)) Settings.IgnoredCells[row, column] ^= 1;

                    if ((counter - 1) % 12 < 11) ImGui.SameLine();

                    counter += 1;
                }
            }
        }
    }
}
