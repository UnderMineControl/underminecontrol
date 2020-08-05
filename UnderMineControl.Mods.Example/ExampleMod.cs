using UnderMineControl.API;
using UnityEngine;

namespace UnderMineControl.Mods.Example
{
    public class ExampleMod : IMod
    {
        #region Injected game objects
        // #################################################################
        // # These are most of the API's objects for manipulating the game #
        // #################################################################

        // This represents the UnderMine game instance
        private readonly IGame _game;

        // This gives access to specific game events
        private readonly IEvents _events;

        // This gives access to player options like health and damage and inventoy
        private readonly IPlayer _player;

        // This allows you to patch game methods using Harmony
        private readonly IPatcher _patcher; 
        #endregion

        // Only pass these objects into the constructor of the mod
        // The order doesn't matter and you only need to pass the ones that you need
        public ExampleMod(IGame game, IEvents events, IPlayer player, IPatcher patcher)
        {
            _game = game;
            _events = events;
            _player = player;
            _patcher = patcher;
        }

        // This method gets run when the game starts up
        public void Initialize()
        {
            
            // This method is the perfect place to hook into any events you might need

            // This event gets fired on a loop, it is what manages most of the core game mechanics.
            _events.OnGameUpdated += OnGameUpdated;
        }

        private void OnGameUpdated(object sender, IGame e)
        {
            // Lets check to see if the F10 key is held down
            // Then lets change the character's name
            // Note: You will have to quit to the main menu for it to change in game!
            if (_game.KeyDown(KeyCode.F10))
                _game.Data.SetPeonName("Doug");

            // Now lets add another key bind for halfing the players HP
            if (_game.KeyDown(KeyCode.F11))
                _player.CurrentHP /= 2;
        }
    }
}
