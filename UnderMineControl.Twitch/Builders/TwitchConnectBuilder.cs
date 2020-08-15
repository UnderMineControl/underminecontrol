using TwitchLib.Client.Models;
using TwitchLib.Unity;

namespace UnderMineControl.Twitch.Builders
{
    public interface ITwitchConnectBuilder 
    {
        ITwitchBot Connect();
    }

    public abstract class TwitchConnectBuilder : ITwitchConnectBuilder
    {
        private TwitchInstance _instance;
        private Client _client => _instance.Client;
        private TwitchCreds _credentials => _instance.Credentials;

        public TwitchConnectBuilder(TwitchInstance instance) 
        {
            _instance = instance;
        }

        public ITwitchBot Connect()
        {
            _client.Connect();

            return new TwitchBot(_instance);
        }
    }
}
