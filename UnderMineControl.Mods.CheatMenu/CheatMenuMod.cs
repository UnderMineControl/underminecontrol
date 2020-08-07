using System;
using System.Collections.Generic;
using System.IO;
using Thor;
using UnityEngine;

namespace UnderMineControl.Mods.CheatMenu
{
    using API;
    using System.Linq;

    public class CheatMenuMod : Mod
    {
        private Dictionary<KeyCode[], Action> _cheatOptions;
        private string Desktop => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public override void Initialize()
        {
            Logger.Debug("Cheat Menu Mod is intializing...");
            _cheatOptions = new Dictionary<KeyCode[], Action>
            {
                [new[] { KeyCode.Alpha1, KeyCode.F1 }] = () => Logger.Debug("Alpha 1 + F1 pressed"),
                [new[] { KeyCode.F1 }] = OpenDoors,
                [new[] { KeyCode.F2 }] = CloseDoors,
                [new[] { KeyCode.F3 }] = () => Player.Invulnerable = !Player.Invulnerable,
                [new[] { KeyCode.F4 }] = () => Player.MaxHP = Player.CurrentHP = 1500,
                [new[] { KeyCode.F5 }] = () => Player.AddRandomBlessing(),
                [new[] { KeyCode.F6 }] = () => { Player.RemoveRandomCurse(out _); Player.RemoveRandomCurse(out _, HealthExt.CurseType.Major); },
                [new[] { KeyCode.F7 }] = () => Player.Bombs = Player.Keys = Player.Gold = Player.Thorium = 10000,
                [new[] { KeyCode.F8 }] = () => Player.Gold *= 2,
                [new[] { KeyCode.F9 }] = Print
            };
            Events.OnGameUpdated += OnGameUpdated;
        }

        private void OnGameUpdated(object sender, IGame e)
        {
            var ops = _cheatOptions.OrderByDescending(t => t.Key.Length);
            foreach(var op in ops)
            {
                if (!AllHeld(op.Key))
                    continue;

                op.Value();
                break;
            }
        }

        private bool AllHeld(params KeyCode[] keys)
        {
            foreach (var key in keys)
                if (!GameInstance.KeyDown(key))
                    return false;

            return true;
        }

        private void Print()
        {
            PrintIds();
            PrintEffects();
        }

        private void PrintIds()
        {
            try
            {
                var path = Path.Combine(Desktop, "items.csv");
                Logger.Debug("Starting to write out all items");
                using (var io = File.Open(path, FileMode.Create))
                using (var sw = new StreamWriter(io))
                {
                    sw.WriteLine("Id,Name,Display Id,Display Name,Description,Is Blessing,Is Curse,Is Minor,Is Hex,Is Special Drop,Is Special Discovery,Audio,Is Deprecated,Is Default,Is Default Discovered,Allow On Altar,Slot,Hint");

                    var items = GameData.Instance.Items;
                    foreach (var item in items)
                    {
                        var parts = new object[]
                        {
                            item.guid,
                            item.name,
                            item.DisplayName.Id,
                            item.DisplayName.Text,
                            item.Description.Text,
                            item.IsBlessing,
                            item.IsCurse,
                            item.IsMinor,
                            item.IsHex,
                            item.IsSpecialDrop,
                            item.IsSpecialDiscovery,
                            item.Audio,
                            item.IsDeprecated,
                            item.IsDefault,
                            item.IsDefaultDiscovered,
                            item.AllowOnAltar,
                            item.Slot,
                            item.Hint
                        };

                        WriteCsvLine(parts, sw);
                    }

                    sw.Flush();
                }
                Logger.Debug("Finished writing out all items");
            }
            catch (Exception ex)
            {
                Logger.Error("Error writing out all items: " + ex);
            }
        }

        private void PrintEffects()
        {
            try
            {
                var path = Path.Combine(Desktop, "effects.csv");
                Logger.Debug("Starting to write out all effects");
                using (var io = File.Open(path, FileMode.Create))
                using (var sw = new StreamWriter(io))
                {
                    sw.WriteLine("Entity.Id,name,IsBlessing,IsCurse,IsMinor,IsHex,Level,MaxDuration,DefaultDuration,Sticky,StackPolicy,Position");

                    var items = GameData.Instance.StatusEffects;
                    foreach (var item in items)
                    {
                        var parts = new object[]
                        {
                            item.Entity.Guid,
                            item.name,
                            item.IsBlessing,
                            item.IsCurse,
                            item.IsMinor,
                            item.IsHex,
                            item.Level,
                            item.MaxDuration,
                            item.DefaultDuration,
                            item.Sticky,
                            item.StackPolicy,
                            item.Position
                        };

                        WriteCsvLine(parts, sw);
                    }

                    sw.Flush();
                }
                Logger.Debug("Finished writing out all effects");
            }
            catch (Exception ex)
            {
                Logger.Error("Error writing out all effects: " + ex);
            }
        }

        private void WriteCsvLine(object[] data, StreamWriter writer)
        {
            var actData = data.Select(t => CsvEscape(t.ToString()));
            var line = string.Join(",", actData.ToArray());
            writer.WriteLine(line);
        }

        private string CsvEscape(string value)
        {
            var needsEscape = value.Contains("\"") ||
                value.Contains("\r") ||
                value.Contains("\n") ||
                value.Contains(",");

            if (!needsEscape)
                return value;

            value = value.Replace("\"", "\"\"");

            return $"\"{value}\"";
        }

        private void OpenDoors()
        {
            GameInstance.Simulation.Zone.CurrentRoom.OpenDoors();
            Logger.Debug("Doors opened");
        }

        private void CloseDoors()
        {
            GameInstance.Simulation.Zone.CurrentRoom.CloseDoors();
            Logger.Debug("Doors closed");
        }
    }
}
