namespace UnderMineControl.API
{
    /// <summary>
    /// Provides access to logging functions.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log level 0: Used to trace the entry and exit of functions for intensive debugging.
        /// </summary>
        /// <param name="message"></param>
        void Trace(string message);

        /// <summary>
        /// Log level 1: Used to provide general information that may be useful to the developer.
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Log level 2: Used to provide general information that may be useful to the end-user.
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Log level 3: Used to document that a problem exists, but will not interfere with the game functionality.
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);

        /// <summary>
        /// Log level 4: Used to document that a problem exists, which will possibly interfere with the game functionality.
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Log level 5: Use sparingly to make the end-user aware of action items that need their involvement, e.g. updates.
        /// </summary>
        /// <param name="message"></param>
        void Alert(string message);

        /// <summary>
        /// Log level 6: Used to document that a problem exists, which will likely prevent the game from continuing.
        /// </summary>
        /// <param name="message"></param>
        void Fatal(string message);
    }
}
