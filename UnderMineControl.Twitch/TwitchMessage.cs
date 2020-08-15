using TwitchLib.Client.Models;

namespace UnderMineControl.Twitch
{
    public interface ITwitchMessage
    {
        ChatCommand ChatCommand { get; }
        WhisperCommand WhisperCommand { get; }
        bool IsWhisper { get; }
        string CommandText { get; }
    }

    public class TwitchMessage : ITwitchMessage
    {
        public ChatCommand ChatCommand { get; set; }
        public WhisperCommand WhisperCommand { get; set; }
        public bool IsWhisper { get; set; }

        public string CommandText => IsWhisper ? WhisperCommand.CommandText : ChatCommand.CommandText;

        public TwitchMessage(ChatCommand chat)
        {
            ChatCommand = chat;
            IsWhisper = false;
        }

        public TwitchMessage(WhisperCommand chat)
        {
            WhisperCommand = chat;
            IsWhisper = true;
        }
    }
}
