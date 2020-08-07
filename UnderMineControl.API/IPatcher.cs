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

        /// <summary>
        /// Gets the value of a specific field
        /// </summary>
        /// <typeparam name="T">The type of the field</typeparam>
        /// <param name="instance">The instance of the object the field is on</param>
        /// <param name="name">The name of the field</param>
        /// <param name="type">The optional reference type to use</param>
        /// <returns>The value of the field</returns>
        T GetField<T>(object instance, string name, Type type = null);

        /// <summary>
        /// Sets the value of a specific field
        /// </summary>
        /// <param name="instance">The instance of the object the field is on</param>
        /// <param name="name">The name of the field</param>
        /// <param name="value">The value to set the field to</param>
        /// <param name="type">The optional type of the field</param>
        void SetField(object instance, string name, object value, Type type = null);

        /// <summary>
        /// Gets the value of a specific property
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="instance">The instance of the object the property is on</param>
        /// <param name="name">The name of the property</param>
        /// <param name="type">The optional reference type to use</param>
        /// <returns>The value of the property</returns>
        T GetProperty<T>(object instance, string name, Type type = null);

        /// <summary>
        /// Sets the value of a specific property
        /// </summary>
        /// <param name="instance">The instance of the object the property is on</param>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="type">The optional type of the property</param>
        void SetProperty(object instance, string name, object value, Type type = null);

        /// <summary>
        /// Invokes a method on an object
        /// </summary>
        /// <typeparam name="T">The return type of the method</typeparam>
        /// <param name="instance">The object the method is on</param>
        /// <param name="name">The name of the method</param>
        /// <param name="type">The optional type of the object</param>
        /// <returns>The methods return result</returns>
        T Invoke<T>(object instance, string name, Type type = null);

        /// <summary>
        /// Invoke a method on an object
        /// </summary>
        /// <param name="instance">The object the method is on</param>
        /// <param name="name">The name of the method</param>
        /// <param name="type">The optional type of the object</param>
        void Invoke(object instance, string name, Type type = null);
    }
}
