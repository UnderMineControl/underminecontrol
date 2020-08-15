using System;
using System.Collections.Generic;
using System.Reflection;
using TwitchLib.Client.Models;

namespace UnderMineControl.Twitch
{
    using API;
    using Commands;
    using Exceptions;

    public interface ITwitchBot
    {
        ITwitchBot Plugin<T>() where T : TwitchPlugin;
        ITwitchBot Plugin(Type type);

        ITwitchBot Whisper(string command,
            Action<ITwitchMessage, ITwitchInstance> action,
            string description = null);

        ITwitchBot Command(string command,
            Action<ITwitchMessage, ITwitchInstance> action,
            string description = null,
            FilterType filter = FilterType.All);
    }

    public class TwitchBot : ITwitchBot
    {
        private TwitchInstance _instance;
        private List<TwitchCommand> _chatCommands => _instance.Commands;
        private ILogger _logger => _instance.Mod.Logger;

        public TwitchBot(TwitchInstance instance)
        {
            _instance = instance;
            _instance.Bot = this;
        }

        public void HandleCommand(ITwitchMessage message)
        {
            foreach(var command in _chatCommands)
            {
                if (command.IsWhisper != message.IsWhisper || 
                    command.Command.ToLower() != message.CommandText.ToLower())
                    continue;

                if (command.IsWhisper)
                {
                    if (!(command is TwitchWhisperCommand whisper))
                        throw new InvalidCommandTypeException("Command type marked as whisper, but not of type TwitchWhisperCommand");

                    if (HandleWhisper(whisper, message))
                        return;

                    continue;
                }

                if (!(command is TwitchChatCommand chat))
                    throw new InvalidCommandTypeException("Command type marked as not whisper, but not of type TwitchChatCommand");

                if (HandleCommand(chat, message))
                    return;
            }
        }

        public bool HandleWhisper(TwitchWhisperCommand command, ITwitchMessage message)
        {
            try
            {
                command.Action(message, _instance);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while handling whisper: " + ex);
                return false;
            }
        }

        public bool HandleFilter(FilterType type, ChatMessage command)
        {
            if (type.HasFlag(FilterType.Users))
                return true;

            if (type.HasFlag(FilterType.Subscribers) &&
                (command.IsBroadcaster || command.IsModerator || command.IsSubscriber))
                return true;

            if (type.HasFlag(FilterType.Moderators) &&
                (command.IsBroadcaster || command.IsModerator))
                return true;

            if (type.HasFlag(FilterType.Broadcaster) && command.IsBroadcaster)
                return true;

            return false;
        }

        public bool HandleCommand(TwitchChatCommand command, ITwitchMessage message)
        {
            try
            {
                if (command.Filter != FilterType.Custom && 
                    !HandleFilter(command.Filter, message.ChatCommand.ChatMessage))
                    return false;

                if (command.Filter == FilterType.Custom &&
                    !command.CustomFilter(message.ChatCommand, this))
                    return false;

                command.Action(message, _instance);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while handling chat: " + ex);
                return false;
            }
        }

        public ITwitchBot Plugin<T>() where T : TwitchPlugin
        {
            return Plugin(typeof(T));
        }

        public ITwitchBot Plugin(Type type)
        {
            var pluginType = typeof(TwitchPlugin);

            if (!pluginType.IsAssignableFrom(type))
                throw new InvalidCastException("Given type is not of type TwitchPlugin: " + type.Name);


            var methods = type.GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attrs = method.GetCustomAttributes<TwitchPluginAttribute>();
                foreach (var atr in attrs)
                {
                    CreateCommand(atr, method, type);
                }
            }

            return this;
        }

        public ITwitchBot Command(string command, 
            Action<ITwitchMessage, ITwitchInstance> action, 
            string description = null,
            FilterType filter = FilterType.All)
        {

            _chatCommands.Add(new TwitchChatCommand
            {
                Command = command,
                Action = action,
                Description = description,
                Filter = filter,
                CustomFilter = null
            });

            return this;
        }

        public ITwitchBot Whisper(string command,
            Action<ITwitchMessage, ITwitchInstance> action,
            string description = null)
        {

            _chatCommands.Add(new TwitchWhisperCommand
            {
                Command = command,
                Action = action,
                Description = description
            });

            return this;
        }

        public void CreateCommand(TwitchPluginAttribute attribute, MethodInfo method, Type type)
        {
            try
            {
                
                var command = GetCommand(attribute);

                command.Action = (m, t) =>
                {
                    try
                    {
                        var parent = (TwitchPlugin)Activator.CreateInstance(type);
                        parent.Message = m;
                        parent.Twitch = t;
                        method.Invoke(parent, new object[0]);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error occurred while running plugin {type.Name}:{method.Name} >> {ex}");
                    }
                };

                _chatCommands.Add(command);
                _logger.Debug("Added a new twitch command: " + attribute.Command);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while creating twitch command: " + ex);
            }
        }

        public TwitchCommand GetCommand(TwitchPluginAttribute attribute)
        {
            if (attribute.IsWhisper)
                return new TwitchWhisperCommand
                {
                    Command = attribute.Command,
                    Description = attribute.Description
                };

            return new TwitchChatCommand
            {
                Command = attribute.Command,
                Description = attribute.Description,
                Filter = attribute.Filter
            };
        }
    }
}
