using System;
using System.Collections.Generic;
using Thor;
using UnityEngine;

namespace UnderMineControl.Mods.CheatMenu
{
    using API;

    public class CheatMenuMod : IMod
    {
        private readonly IGame _game;
        private readonly API.ILogger _logger;
        private readonly IEvents _events;
        private readonly IPlayer _player;

        private Dictionary<KeyCode, Action> _cheatOptions;

        public CheatMenuMod(IGame game, API.ILogger logger, IEvents events, IPlayer player)
        {
            _game = game;
            _logger = logger;
            _events = events;
            _player = player;
        }

        public void Initialize()
        {
            _logger.Debug("Cheat Menu Mod is intializing...");
            _cheatOptions = new Dictionary<KeyCode, Action>
            {
                [KeyCode.F1] = OpenDoors,
                [KeyCode.F2] = CloseDoors,
                [KeyCode.F3] = () => _player.Invulnerable = !_player.Invulnerable,
                [KeyCode.F4] = () => _player.MaxHP = _player.CurrentHP = 1500,
                [KeyCode.F5] = () => _player.AddRandomBlessing(),
                [KeyCode.F6] = () => _player.Bombs = _player.Keys = _player.Gold = _player.Thorium = 1000
            };
            _events.OnGameUpdated += OnGameUpdated;
        }

        private void OnGameUpdated(object sender, IGame e)
        {
            foreach(var op in _cheatOptions)
            {
                if (!_game.KeyDown(op.Key))
                    continue;

                op.Value();
                break;
            }
        }

        private void OpenDoors()
        {
            Game.Instance.Simulation.Zone.CurrentRoom.OpenDoors();
            _logger.Debug("Doors opened");
        }

        private void CloseDoors()
        {
            Game.Instance.Simulation.Zone.CurrentRoom.CloseDoors();
            _logger.Debug("Doors closed");
        }
    }
}
