using HarmonyLib;
using System;
using System.Reflection;

namespace UnderMineControl.Utility
{
    using API;

    /// <summary>
    /// An implementation of the <see cref="IPatcher"/> interface
    /// Used for patching runtime methods with Harmony
    /// </summary>
    public class Patcher : IPatcher
    {
        private const BindingFlags BindingFlagsAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// The Harmony instance
        /// </summary>
        private Harmony _harmony;
        /// <summary>
        /// The logger instance
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="harmony">The harmony instance</param>
        /// <param name="logger">The logger instance</param>
        public Patcher(Harmony harmony, ILogger logger)
        {
            _harmony = harmony;
            _logger = logger;
        }

        /// <summary>
        /// Patches the given method with Harmony (pre and post fix)
        /// </summary>
        /// <param name="caller">The class the patched methods are on (required)</param>
        /// <param name="target">The class the methods to patch are on (required)</param>
        /// <param name="method">What method needs patching (required)</param>
        /// <param name="prefix">What method to use as the prefix (optional)</param>
        /// <param name="postfix">What method to use as the postfix (optional)</param>
        /// <param name="parameters">Any parameters necessary for the target method (optional)</param>
        /// <returns>Whether or not method was successfully patched or throws <see cref="NullReferenceException"/> if required parameters are missing</returns>
        public bool Patch(object caller, Type target, string method, string prefix, string postfix, params Type[] parameters)
        {
            //Check our required parameters
            if (caller == null)
                throw new NullReferenceException("caller");
            if (target == null)
                throw new NullReferenceException("target");
            if (string.IsNullOrEmpty(method))
                throw new NullReferenceException("method");

            //Fetch the target method
            var targetMethod = AccessTools.Method(target, method, parameters);
            if (targetMethod == null) //Verify target method exists
            {
                _logger.Error($"Target method doesn't exist: {target.Name}::{method}");
                return false;
            }
            //Make sure the caller is a Type, if not: fetch the type
            var type = (caller is Type t) ? t : caller.GetType();
            //Patch the method
            return Patch(type, targetMethod, prefix, postfix);
        }

        /// <summary>
        /// Patches the given method with Harmony (pre and post fix)
        /// </summary>
        /// <param name="caller">The type of the class the patched methods are on (required)</param>
        /// <param name="target">The method we want to patch (required)</param>
        /// <param name="prefix">What method to use as the prefix (optional)</param>
        /// <param name="postfix">What method to use as the postfix (optional)</param>
        /// <returns>Whether or not method was successfully patched or throws <see cref="NullReferenceException"/> if required parameters are missing</returns>
        public bool Patch(Type caller, MethodInfo target, string prefix, string postfix)
        {
            //Check our required parameters
            if (caller == null)
                throw new NullReferenceException("caller");
            if (target == null)
                throw new NullReferenceException("target");

            //Generate any necessary patches
            HarmonyMethod pre = string.IsNullOrEmpty(prefix) ? null : GetHarmonyMethod(caller, prefix),
                          pos = string.IsNullOrEmpty(postfix) ? null : GetHarmonyMethod(caller, postfix);

            //Verify we have something to patch
            if (pre == null && pos == null)
            {
                //We don't have anything to patch, notify and return.
                _logger.Warn("Nothing to patch, both prefix and postfix are null");
                return false;
            }

            try
            {
                //Patch the method using Harmony
                var method = _harmony.Patch(target, pre, pos);

                //Everything succeeded. Notify and return.
                _logger.Debug($"Succeeded to patch methods: {caller.Name}::{target.Name} ({pre?.methodName}|{pos?.methodName})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to patch any methods for {caller.Name}::{target.Name} ({prefix}|{postfix})\r\n{ex}");
                return false;
            }
        }

        /// <summary>
        /// Generates the harmony method from a given class and method
        /// </summary>
        /// <param name="caller">The type of the class the method is on (required)</param>
        /// <param name="method">The name of the method we want to generate a patch for (required)</param>
        /// <returns>The generated harmony method, null if it was unsuccessful, or throws <see cref="NullReferenceException"/> if required parameters are missing</returns>
        public HarmonyMethod GetHarmonyMethod(Type caller, string method)
        {
            if (caller == null)
                throw new NullReferenceException("caller");
            if (string.IsNullOrEmpty(method))
                throw new NullReferenceException("method");

            //Get the method that matches the method name
            var flags = (BindingFlags)(-1); //Trick to force reflection to find the matching method with any binding flags
            var methodInfo = caller.GetMethod(method, flags);
            //Verify our method info is present
            if (methodInfo == null)
            {
                //It's not, notify and return.
                _logger.Error($"Harmony method doesn't exist: {caller.Name}::{method}");
                return null;
            }

            //Generate the harmony method
            var hm = new HarmonyMethod(methodInfo);
            //Verify our method was generated
            if (hm == null)
            {
                //It's not, notify and return.
                _logger.Error($"Error patching method: {caller.Name}::{method}");
                return null;
            }

            //We did it! Yay!
            return hm;
        }

        /// <summary>
        /// Gets the value of a specific field
        /// </summary>
        /// <typeparam name="T">The type of the field</typeparam>
        /// <param name="instance">The instance of the object the field is on</param>
        /// <param name="name">The name of the field</param>
        /// <param name="type">The optional reference type to use</param>
        /// <returns>The value of the field</returns>
        public T GetField<T>(object instance, string name, Type type = null)
        {
            Type t = (type == null ? instance.GetType() : type);
            return (T)t.GetField(name, BindingFlagsAll).GetValue(instance);
        }

        /// <summary>
        /// Sets the value of a specific field
        /// </summary>
        /// <param name="instance">The instance of the object the field is on</param>
        /// <param name="name">The name of the field</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="type">The optional type of the field</param>
        public void SetField(object instance, string name, object value, Type type = null)
        {
            Type t = (type == null ? instance.GetType() : type);
            t.GetField(name, BindingFlagsAll).SetValue(instance, value);
        }

        /// <summary>
        /// Gets the value of a specific property
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="instance">The instance of the object the property is on</param>
        /// <param name="name">The name of the property</param>
        /// <param name="type">The optional reference type to use</param>
        /// <returns>The value of the property</returns>
        public T GetProperty<T>(object instance, string name, Type type = null)
        {
            Type t = (type == null ? instance.GetType() : type);
            return (T)t.GetProperty(name, BindingFlagsAll).GetValue(instance);
        }

        /// <summary>
        /// Sets the value of a specific property
        /// </summary>
        /// <param name="instance">The instance of the object the property is on</param>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="type">The optional type of the property</param>
        public void SetProperty(object instance, string name, object value, Type type = null)
        {
            Type t = (type == null ? instance.GetType() : type);
            t.GetProperty(name, BindingFlagsAll).SetValue(instance, value);
        }

        /// <summary>
        /// Invokes a method on an object
        /// </summary>
        /// <typeparam name="T">The return type of the method</typeparam>
        /// <param name="instance">The object the method is on</param>
        /// <param name="name">The name of the method</param>
        /// <param name="type">The optional type of the object</param>
        /// <returns>The methods return result</returns>
        public T Invoke<T>(object instance, string name, Type type = null)
        {
            Type t = (type == null ? instance.GetType() : type);
            return (T)t.GetMethod(name, BindingFlagsAll).Invoke(instance, null);
        }

        /// <summary>
        /// Invoke a method on an object
        /// </summary>
        /// <param name="instance">The object the method is on</param>
        /// <param name="name">The name of the method</param>
        /// <param name="type">The optional type of the object</param>
        public void Invoke(object instance, string name, Type type = null)
        {
            Type t = (type == null ? instance.GetType() : type);
            t.GetMethod(name, BindingFlagsAll).Invoke(instance, null);
        }
    }
}

