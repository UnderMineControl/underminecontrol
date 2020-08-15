using System.Collections.Generic;
using TwitchLib.Unity;

namespace UnderMineControl.Twitch
{
    using API;
    using Builders;
    using Commands;

    public static class Extensions
    {
        public static ITwitchCredentialBuilder AddTwitch(this Mod mod)
        {
            var instance = new TwitchInstance
            {
                Client = new Client(),
                Commands = new List<TwitchCommand>(),
                Credentials = null,
                Events = null,
                Bot = null,
                Mod = mod
            };

            return new TwitchCredentialBuilder(instance);
        }
    }
}
