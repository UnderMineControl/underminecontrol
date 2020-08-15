using System.Collections.Generic;
using TwitchLib.Unity;

namespace UnderMineControl.Twitch
{
    using API;
    using Builders;
    using Commands;

    public interface ITwitchInstance
    {
        Client Client { get; }
        TwitchCreds Credentials { get; }
        List<TwitchCommand> Commands { get; }
        TwitchEventBuilder Events { get; }
    }

    public class TwitchInstance : ITwitchInstance
    {
        public Client Client { get; set; }
        public Mod Mod { get; set; }
        public TwitchCreds Credentials { get; set; }
        public List<TwitchCommand> Commands { get; set; }
        public TwitchEventBuilder Events { get; set; }
        public TwitchBot Bot { get; set; }
    }
}
