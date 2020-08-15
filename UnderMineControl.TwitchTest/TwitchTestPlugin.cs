namespace UnderMineControl.TwitchTest
{
    using Twitch;

    public class TwitchTestPlugin : TwitchPlugin
    {
        [ChatPlugin("hello", "This is a test")]
        public void Test()
        {
            Logger.Debug("hello hit");
            Reply("How are you today?");
        }
    }
}
