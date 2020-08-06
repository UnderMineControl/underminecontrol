using System;
using System.Collections.Generic;
using Thor;
using UnityEngine;

namespace UnderMineControl.Mods.CheatMenu
{
    using API;

    public class CheatMenuMod : Mod
    {
        private Dictionary<KeyCode, Action> _cheatOptions;

        public override void Initialize()
        {
            Logger.Debug("Cheat Menu Mod is intializing...");
            _cheatOptions = new Dictionary<KeyCode, Action>
            {
                [KeyCode.F1] = OpenDoors,
                [KeyCode.F2] = CloseDoors,
                [KeyCode.F3] = () => Player.Invulnerable = !Player.Invulnerable,
                [KeyCode.F4] = () => Player.MaxHP = Player.CurrentHP = 1500,
                [KeyCode.F5] = () => Player.AddRandomBlessing(),
                [KeyCode.F6] = () => { Player.RemoveRandomCurse(out _); Player.RemoveRandomCurse(out _, HealthExt.CurseType.Major); },
                [KeyCode.F7] = () => Player.Bombs = Player.Keys = Player.Gold = Player.Thorium = 10000,
                [KeyCode.F8] = () => Player.Gold *= 2
            };
            Events.OnGameUpdated += OnGameUpdated;
        }

        private void OnGameUpdated(object sender, IGame e)
        {
            foreach(var op in _cheatOptions)
            {
                if (!GameInstance.KeyDown(op.Key))
                    continue;

                op.Value();
                break;
            }
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
