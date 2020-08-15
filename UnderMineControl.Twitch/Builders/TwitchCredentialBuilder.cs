using System;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

namespace UnderMineControl.Twitch.Builders
{
    public interface ITwitchCredentialBuilder : ITwitchConnectBuilder
    {
        ITwitchEventBuilder Credentials(TwitchCreds creds);
        ITwitchEventBuilder Credentials(string username, string oAuthToken, string clientId = null, char commandChar = '!', string[] channels = null);
        ITwitchEventBuilder Credentials(string configFileName);
    }

    public class TwitchCredentialBuilder : TwitchConnectBuilder, ITwitchCredentialBuilder
    {
        private TwitchInstance _instance;
        private Client _client => _instance.Client;
        private TwitchCreds _credentials => _instance.Credentials;

        public TwitchCredentialBuilder(TwitchInstance instance) : base(instance)
        {
            _instance = instance;
        }

        public ITwitchEventBuilder Credentials(TwitchCreds creds)
        {
            _instance.Credentials = creds;

            var credentials = new ConnectionCredentials(_credentials.Username, _credentials.OAuthToken);
            _client.Initialize(credentials, null, _credentials.CommandCharacter, _credentials.CommandCharacter);

            if (string.IsNullOrEmpty(creds.Username))
                throw new ArgumentNullException("Username", "Twitch username cannot be null! This is the username of the account the bot will be signed in on!");

            if (string.IsNullOrEmpty(creds.OAuthToken))
                throw new ArgumentNullException("OAuthToken", "Twitch OAuthToken cannot be null! This is the Access Token for you account and can be found: https://twitchtokengenerator.com/");

            return new TwitchEventBuilder(_instance);
        }

        public ITwitchEventBuilder Credentials(string username, string oAuthToken, string clientId = null, char commandChar = '!', string[] channels = null)
        {
            return Credentials(new TwitchCreds
            {
                Username = username,
                OAuthToken = oAuthToken,
                ClientId = clientId,
                CommandCharacter = commandChar,
                Channels = channels ?? new string[0]
            });
        }

        public ITwitchEventBuilder Credentials(string configFileName)
        {
            var creds = _instance.Mod.Configuration.Get<TwitchCreds>(configFileName);
            return Credentials(creds);
        }
    }
}
