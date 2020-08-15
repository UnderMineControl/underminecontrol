using System;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.Unity;

namespace UnderMineControl.Twitch.Builders
{
    using API;

    public interface ITwitchEventBuilder : ITwitchConnectBuilder
    {
        event EventHandler<OnConnectedArgs> OnTwitchConnected;
        event EventHandler<OnJoinedChannelArgs> OnTwitchJoinedChannel;
        event EventHandler<ITwitchMessage> OnTwitchCommand;
        event EventHandler<OnConnectionErrorArgs> OnTwitchConnectionError;
        event EventHandler<OnDisconnectedEventArgs> OnTwitchDisconnected;

        ITwitchEventBuilder OnConnected(Action<OnConnectedArgs> action);
        ITwitchEventBuilder OnDisconnected(Action<OnDisconnectedEventArgs> action);
        ITwitchEventBuilder OnCommand(Action<ITwitchMessage> action);
        ITwitchEventBuilder OnConnectError(Action<OnConnectionErrorArgs> action);
        ITwitchEventBuilder OnChannelJoined(Action<OnJoinedChannelArgs> action);
    }

    public class TwitchEventBuilder : TwitchConnectBuilder, ITwitchEventBuilder
    {
        public event EventHandler<OnConnectedArgs> OnTwitchConnected = delegate { };
        public event EventHandler<OnJoinedChannelArgs> OnTwitchJoinedChannel = delegate { };
        public event EventHandler<ITwitchMessage> OnTwitchCommand = delegate { };
        public event EventHandler<OnConnectionErrorArgs> OnTwitchConnectionError = delegate { };
        public event EventHandler<OnDisconnectedEventArgs> OnTwitchDisconnected = delegate { };

        private TwitchInstance _instance;
        private Client _client => _instance.Client;
        private ILogger _logger => _instance.Mod.Logger;
        private TwitchCreds _credentials => _instance.Credentials;

        public TwitchEventBuilder(TwitchInstance instance) : base(instance)
        {
            _instance = instance;
            _instance.Events = this;

            if (_instance.Client == null)
                throw new ArgumentNullException("instance.Client", "Client cannot be null!");

            Initialize();
        }

        public void Initialize()
        {
            _client.OnConnected += OnTwitchConnected_Event;
            _client.OnJoinedChannel += OnTwitchChannelJoin_Event;
            _client.OnChatCommandReceived += (s, e) => OnTwitchCommand_Event(s, new TwitchMessage(e.Command));
            _client.OnWhisperCommandReceived += (s, e) => OnTwitchCommand_Event(s, new TwitchMessage(e.Command));
            _client.OnConnectionError += OnTwitchConnectionError_Event;
            _client.OnDisconnected += OnTwitchDisconnected_Event;
        }

        public ITwitchEventBuilder OnConnected(Action<OnConnectedArgs> action)
        {
            if (action == null)
                return this;

            OnTwitchConnected += (s, e) => action(e);
            return this;
        }

        public ITwitchEventBuilder OnDisconnected(Action<OnDisconnectedEventArgs> action)
        {
            if (action == null)
                return this;

            OnTwitchDisconnected += (s, e) => action(e);
            return this;
        }

        public ITwitchEventBuilder OnCommand(Action<ITwitchMessage> action)
        {
            if (action == null)
                return this;

            OnTwitchCommand += (s, e) => action(e);
            return this;
        }

        public ITwitchEventBuilder OnConnectError(Action<OnConnectionErrorArgs> action)
        {
            if (action == null)
                return this;

            OnTwitchConnectionError += (s, e) => action(e);
            return this;
        }

        public ITwitchEventBuilder OnChannelJoined(Action<OnJoinedChannelArgs> action)
        {
            if (action == null)
                return this;

            OnTwitchJoinedChannel += (s, e) => action(e);
            return this;
        }

        private void OnTwitchCommand_Event(object sender, ITwitchMessage message)
        {
            _instance.Bot?.HandleCommand(message);
            OnTwitchCommand(sender, message);
        }

        private void OnTwitchDisconnected_Event(object sender, OnDisconnectedEventArgs e)
        {
            _logger.Warn("Twitch client disconnected!");
            OnTwitchDisconnected(sender, e);
        }

        private void OnTwitchConnectionError_Event(object sender, OnConnectionErrorArgs e)
        {
            _logger.Error("Error occurred while connecting to twitch: " + e.Error.Message);
            OnTwitchConnectionError(sender, e);
        }

        private void OnTwitchChannelJoin_Event(object sender, OnJoinedChannelArgs e)
        {
            OnTwitchJoinedChannel(sender, e);
        }

        private void OnTwitchConnected_Event(object sender, OnConnectedArgs e)
        {
            _logger.Debug("Twitch client connected!");
            if (_credentials.Channels != null &&
                _credentials.Channels.Length > 0)
                foreach (var channel in _credentials.Channels)
                    _client.JoinChannel(channel);

            OnTwitchConnected(sender, e);
        }
    }
}
