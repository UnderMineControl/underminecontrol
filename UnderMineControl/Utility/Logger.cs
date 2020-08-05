namespace UnderMineControl.Utility
{
    using API;

    /// <summary>
    /// An implementation of the <see cref="ILogger"/> interface used to log entries to the Unity Engine console
    /// </summary>
    public class Logger : ILogger
    {
        private const string APP_NAME = "UnderMineControl";

        /// <summary>
        /// The different log levels available
        /// </summary>
        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Alert = 5,
            Fatal = 6
        }


        /// <summary>
        /// Logs a message to the <see cref="UnityEngine.Debug"/> console
        /// </summary>
        /// <param name="level">The level to log at (changes the color)</param>
        /// <param name="message">The message to log</param>
        public void Log(LogLevel level, string message)
        {
            var msg = $"[{APP_NAME}::{level}] {message}";
            switch (level)
            {
                default:
                    UnityEngine.Debug.Log(msg);
                    break;
                case LogLevel.Warn:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                case LogLevel.Alert:
                    UnityEngine.Debug.LogError(msg);
                    break;
            }
        }

        public void Alert(string message)
        {
            Log(LogLevel.Alert, message);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }
    }
}
