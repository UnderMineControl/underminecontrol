namespace UnderMineControl.Twitch
{
    using API;
    using API.Models;

    public abstract class TwitchPlugin
    {
        /// <summary>
        /// Access to the message that was just sent
        /// </summary>
        public ITwitchMessage Message { get; set; }
        /// <summary>
        /// Access to everything else twitch
        /// </summary>
        public ITwitchInstance Twitch { get; set; }
        /// <summary>
        /// Access to the mod that triggered this plugin
        /// </summary>
        public Mod Mod => ((TwitchInstance)Twitch).Mod;
        /// <summary>
        /// This represents the instance of the game
        /// </summary>
        public IGame GameInstance => Mod.GameInstance;
        /// <summary>
        /// Provides access to logging functions
        /// </summary>
        public ILogger Logger => Mod.Logger;
        /// <summary>
        /// Used to patch game methods using Harmony
        /// </summary>
        public IPatcher Patcher => Mod.Patcher;
        /// <summary>
        /// This represents the instance of the player
        /// </summary>
        public IPlayer Player => Mod.Player;
        /// <summary>
        /// Gets the mod's information
        /// </summary>
        public IMod ModData => Mod.ModData;
        /// <summary>
        /// Allows access to resources embedded within your mod
        /// </summary>
        public IResourceUtility Resources => Mod.Resources;
        /// <summary>
        /// Allows access to configuration files
        /// </summary>
        public IConfiguration Configuration => Mod.Configuration;
        /// <summary>
        /// Access to the bot and some helpful methods
        /// </summary>
        public ITwitchBot Bot => ((TwitchInstance)Twitch).Bot;

        /// <summary>
        /// Replies to the message that was sent. 
        /// </summary>
        /// <param name="message">The message to reply with</param>
        /// <param name="dryRun">Whether or not it's a dry run</param>
        public virtual void Reply(string message, bool dryRun = false)
        {
            if (Message.IsWhisper)
            {
                Twitch.Client.SendWhisper(Message.WhisperCommand.WhisperMessage.UserId, message, dryRun);
                return;
            }

            var channel = Twitch.Client.GetJoinedChannel(Message.ChatCommand.ChatMessage.RoomId);
            Twitch.Client.SendMessage(channel, message, dryRun);
        }

        /// <summary>
        /// Sends a whisper to the user that sent the command
        /// </summary>
        /// <param name="message">The message to reply with</param>
        /// <param name="dryRun">Whether or not it's a dry run</param>
        public virtual void Whisper(string message, bool dryRun = false)
        {
            if (Message.IsWhisper)
            {
                Reply(message, dryRun);
                return;
            }

            var userid = Message.ChatCommand.ChatMessage.UserId;
            Twitch.Client.SendWhisper(userid, message, dryRun);
        }
    }
}
