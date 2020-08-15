using System;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace UnderMineControl.TwitchTest
{
    using API;
    using Twitch;

    public class TwitchMod : Mod
    {
        private ITwitchBot _bot;
        private Client _client;

        public override void Initialize()
        {
            try
            {
                var creds = new ConnectionCredentials("cardboardknight_bot", "7vz1t6kfs7p9tskoq0qlfgr556dr7o");

                _client = new Client();

                _client.Initialize(creds, "cardboard_mf");

                _client.OnConnected += _client_OnConnected;
                _client.OnJoinedChannel += _client_OnJoinedChannel;
                _client.OnMessageReceived += _client_OnMessageReceived;
                _client.OnDisconnected += _client_OnDisconnected1;
                _client.OnNoPermissionError += _client_OnNoPermissionError;
                _client.OnConnectionError += _client_OnConnectionError;
                _client.OnError += _client_OnError;

                _client.Connect();

                Events.OnGameUpdated += Events_OnGameUpdated;
                Logger.Debug("Hello");
            }
            catch (Exception ex)
            {
                Logger.Error("Error occurred: " + ex);
            }
        }

        private void _client_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
        {
            Logger.Error("Error: " + e.Exception);
        }

        private void _client_OnDisconnected1(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            Logger.Warn("Disconnected");
        }

        private void _client_OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            Logger.Warn("Connection error");
        }

        private void Events_OnGameUpdated(object sender, IGame e)
        {
            if (e.KeyDown(KeyCode.F2))
                _client.SendMessage("cardboard_mf", "hello world");
        }

        private void _client_OnNoPermissionError(object sender, System.EventArgs e)
        {
            Logger.Warn("Permissions error");
        }

        private void _client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Logger.Debug("Message: " + e.ChatMessage.Message);
        }

        private void _client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
        {
            Logger.Debug("Channel Joined");
        }

        private void _client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Logger.Debug("Client connected");
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Logger.Debug("Hit " + e.ChatMessage.Message);
        }
    }
}
