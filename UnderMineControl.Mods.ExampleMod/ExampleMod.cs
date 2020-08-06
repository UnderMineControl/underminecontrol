using UnderMineControl.API;
using UnityEngine;

namespace UnderMineControl.Mods.ExampleMod
{
    public class ExampleMod : Mod
    {
        // This method gets run when the game starts up
        public override void Initialize()
        {
            
            // This method is the perfect place to hook into any events you might need

            // This event gets fired on a loop, it is what manages most of the core game mechanics.
            Events.OnGameUpdated += OnGameUpdated;
        }

        private void OnGameUpdated(object sender, IGame e)
        {
            // Lets check to see if the F10 key is held down
            // Then lets change the character's name
            // Note: You will have to quit to the main menu for it to change in game!
            if (GameInstance.KeyDown(KeyCode.F10))
                GameInstance.Data.SetPeonName("Doug");

            // Now lets add another key bind for halfing the players HP
            if (GameInstance.KeyDown(KeyCode.F11))
                Player.CurrentHP /= 2;
        }
    }
}
