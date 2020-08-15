using System;

namespace UnderMineControl.Twitch.Commands
{
    public abstract class TwitchCommand
    {
        public string Command { get; set; }
        public string Description { get; set; }
        public abstract bool IsWhisper { get; }
        public Action<ITwitchMessage, ITwitchInstance> Action { get; set; }

        public abstract bool IsValid();
    }
}
