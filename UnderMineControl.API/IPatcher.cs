using System;

namespace UnderMineControl.API
{
    /// <summary>
    /// Used to patch game methods using Harmony
    /// </summary>
    public interface IPatcher
    {
        /// <summary>
        /// Patches a method within the target library
        /// </summary>
        /// <param name="caller">The object or type your methods are on</param>
        /// <param name="target">The type the target method is on</param>
        /// <param name="method">The name of the method that needs to be patched</param>
        /// <param name="prefix">The name of the method to use as a prefix patch (optional)</param>
        /// <param name="postfix">The name of the method to use as a postfix patch (optional)</param>
        /// <param name="parameters">Any parameters present on the method to patch</param>
        /// <returns>Whether or not it was patched successfully</returns>
        bool Patch(object caller, Type target, string method, string prefix, string postfix, params Type[] parameters);
    }
}
