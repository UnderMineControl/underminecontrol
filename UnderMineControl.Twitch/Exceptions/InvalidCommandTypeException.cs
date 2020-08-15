using System;

namespace UnderMineControl.Twitch.Exceptions
{
    public class InvalidCommandTypeException : Exception 
    { 
        public InvalidCommandTypeException(string message) : base(message) { }
    }
}
