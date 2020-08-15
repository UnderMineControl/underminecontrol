namespace UnderMineControl.Twitch.Commands
{
    public class TwitchWhisperCommand : TwitchCommand
    {
        public override bool IsWhisper => true;

        public override bool IsValid()
        {
            if (Action == null)
                return false;

            return true;
        }
    }
}
