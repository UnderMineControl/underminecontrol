using System;
using TwitchLib.Client.Models;

namespace UnderMineControl.Twitch.Commands
{
    public class TwitchChatCommand : TwitchCommand
    {
        public override bool IsWhisper => false;
        public FilterType Filter { get; set; }
        public Func<ChatCommand, ITwitchBot, bool> CustomFilter { get; set; }

        public override bool IsValid()
        {
            if (Filter == FilterType.Custom &&
                CustomFilter == null)
                return false;

            if (Filter.HasFlag(FilterType.Custom) &&
                Filter != FilterType.Custom)
                return false;

            if (Action == null)
                return false;

            return true;
        }
    }
}
