using System;

namespace UnderMineControl.Twitch
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class TwitchPluginAttribute : Attribute
    {
        public virtual string Command { get; }
        public virtual string Description { get; }

        public virtual FilterType Filter { get; }
        public abstract bool IsWhisper { get; }

        public TwitchPluginAttribute(string command, string description)
        {
            Command = command;
            Description = description;
        }
    }

    public class ChatPluginAttribute : TwitchPluginAttribute
    {
        public override bool IsWhisper => false;

        public override FilterType Filter => FilterType.All;

        public ChatPluginAttribute(string command, string description) : base(command, description) { }

        public ChatPluginAttribute(string command) : base(command, null) { }
    }

    public class ModChatPluginAttribute : TwitchPluginAttribute
    {
        public override bool IsWhisper => false;

        public override FilterType Filter => FilterType.Moderators;

        public ModChatPluginAttribute(string command, string description) : base(command, description) { }

        public ModChatPluginAttribute(string command) : base(command, null) { }
    }

    public class SubscriberChatPluginAttribute : TwitchPluginAttribute
    {
        public override bool IsWhisper => false;

        public override FilterType Filter => FilterType.Subscribers;

        public SubscriberChatPluginAttribute(string command, string description) : base(command, description) { }

        public SubscriberChatPluginAttribute(string command) : base(command, null) { }
    }
}
