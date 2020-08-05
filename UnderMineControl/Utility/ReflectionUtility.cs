using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnderMineControl.Utility
{
    public interface IReflectionUtility
    {
        /// <summary>
        /// Gets all types that implement the specified type
        /// </summary>
        /// <param name="implementedInterface">The specified type</param>
        /// <returns>All types that implement the specified type</returns>
        IEnumerable<Type> GetTypes(Type implementedInterface, Assembly startAssembly = null);

        /// <summary>
        /// Converts an object of one type to another
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <param name="toType">The type to convert to</param>
        /// <returns>The converted object</returns>
        object ChangeType(object obj, Type toType);

        /// <summary>
        /// Converts an object of one type to another
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted object</returns>
        T ChangeType<T>(object obj);

        /// <summary>
        /// Gets all types that implement a specified type
        /// </summary>
        /// <typeparam name="T">The specified type</typeparam>
        /// <returns>All instances of the specified type</returns>
        IEnumerable<T> GetAllTypesOf<T>();

        /// <summary>
        /// Gets all types that implement a specified type
        /// </summary>
        /// <param name="type">The specified type</param>
        /// <returns>All instances of the specified type</returns>
        IEnumerable<object> GetAllTypesOf(Type type);

        /// <summary>
        /// Creates an instance of the object using dependency injection
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        /// <returns>The created type</returns>
        T GetInstance<T>(bool returnFirst = false);

        /// <summary>
        /// Creates an instance of the object using dependency injection
        /// </summary>
        /// <param name="type">The type to create</param>
        /// <returns>The created type</returns>
        object GetInstance(Type type, bool returnFirst = false);

        /// <summary>
        /// Exectues a method and passes in the arguments specified or attempts to resolve arguments using dependency injection
        /// </summary>
        /// <param name="info">The method to execute</param>
        /// <param name="def">The context in which to execute the method</param>
        /// <param name="error">If an error occurred during execution</param>
        /// <param name="defaultparameters">The parameters that the method could possibly take</param>
        /// <returns>The return object of the method or an exception if "error" is true</returns>
        object ExecuteDynamicMethod(MethodInfo info, object def, out bool error, params object[] defaultparameters);

        /// <summary>
        /// Executes a method using the defined parameters
        /// </summary>
        /// <param name="info">The method to execute</param>
        /// <param name="def">The context in which to execute the method</param>
        /// <param name="error">If an error occurred during execution</param>
        /// <param name="pars">The parameters to execute the method with</param>
        /// <returns>The return object of the method or an exception if "error" is true</returns>
        object ExecuteMethod(MethodInfo info, object def, out bool error, params object[] pars);
    }

    public class ReflectionUtility : IReflectionUtility
    {
        public static Encoding Encoder = Encoding.UTF8;
        private static Dictionary<Type, object> _implementations = new Dictionary<Type, object>();

        public void RegisterObject(object obj)
        {
            if (obj == null)
                return;

            var type = obj.GetType();
            if (_implementations.ContainsKey(type))
                throw new Exception("Type already exists: " + type.Name);

            _implementations.Add(type, obj);
        }

        public void RegisterObject<T>()
        {
            GetInstance<T>();
        }

        public T GetInstance<T>(bool returnFirst = false)
        {
            return (T)GetInstance(typeof(T), returnFirst);
        }

        public object GetInstance(Type type, bool returnFirst = false)
        {
            if (_implementations.ContainsKey(type))
                return _implementations[type];

            var exists = _implementations.Where(t => type.IsAssignableFrom(t.Key))
                                         .ToArray();

            if (exists.Length <= 0)
            {
                var instance = CreateInstance(type);
                _implementations.Add(type, instance);
                return instance;
            }

            if (exists.Length > 1 && !returnFirst)
                throw new Exception("More than 1 type matches this type.");

            return exists.First().Value;
        }

        public object CreateInstance(Type type)
        {
            var ctros = type.GetConstructors()
                            .Select(t => new KeyValuePair<ConstructorInfo, ParameterInfo[]>(t, t.GetParameters()))
                            .OrderBy(t => t.Value.Length)
                            .ToArray();

            if (ctros.Length == 0)
                return Activator.CreateInstance(type);

            var first = ctros[0];

            var pars = first.Value.Select(t => GetInstance(t.ParameterType)).ToArray();
            return Activator.CreateInstance(type, pars);
        }

        public IEnumerable<T> GetAllTypesOf<T>() 
        {
            var type = typeof(T);
            return GetAllTypesOf(type).Select(t => (T)t);
        }

        public IEnumerable<object> GetAllTypesOf(Type type)
        {
            return _implementations.Where(t =>
                                        t.Key.IsAssignableFrom(type) ||
                                        t.Key == type)
                                   .Select(t => t.Value);
        }

        public virtual object ChangeType(object obj, Type toType)
        {
            if (obj == null)
                return null;

            var fromType = obj.GetType();

            var to = Nullable.GetUnderlyingType(toType) ?? toType;
            var from = Nullable.GetUnderlyingType(fromType) ?? fromType;

            if (to == from)
                return obj;

            if (to.IsEnum)
            {
                return Enum.ToObject(to, Convert.ChangeType(obj, to.GetEnumUnderlyingType()));
            }

            if (from == typeof(byte[]) && to == typeof(string))
            {
                return Encoder.GetString((byte[])obj);
            }

            if (to == typeof(byte[]) && from == typeof(string))
            {
                return Encoder.GetBytes((string)obj);
            }

            return Convert.ChangeType(obj, to);
        }

        public virtual T ChangeType<T>(object obj)
        {
            return (T)ChangeType(obj, typeof(T));
        }

        public virtual IEnumerable<Type> GetTypes(Type implementedInterface, Assembly startAssembly = null)
        {
            var assembly = startAssembly ?? Assembly.GetEntryAssembly();
            var alreadyLoaded = new List<string>
            {
                assembly.FullName
            };

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (type.ImplementedInterfaces.Contains(implementedInterface) ||
                    implementedInterface.IsAssignableFrom(type))
                    yield return type;
            }

            var assems = assembly.GetReferencedAssemblies()
                .Select(t => t.FullName)
                .Except(alreadyLoaded)
                .ToArray();
            foreach (var ass in assems)
            {
                foreach (var type in GetTypes(implementedInterface, ass, alreadyLoaded))
                {
                    yield return type;
                }
            }
        }

        private IEnumerable<Type> GetTypes(Type implementedInterface, string assembly, List<string> alreadyLoaded)
        {
            if (alreadyLoaded.Contains(assembly))
                yield break;

            alreadyLoaded.Add(assembly);
            var asml = Assembly.Load(assembly);
            foreach (var type in asml.DefinedTypes)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (type.ImplementedInterfaces.Contains(implementedInterface) ||
                    implementedInterface.IsAssignableFrom(type))
                    yield return type;
            }

            var assems = asml.GetReferencedAssemblies()
                .Select(t => t.FullName)
                .Except(alreadyLoaded)
                .ToArray();
            foreach (var ass in assems)
            {
                foreach (var type in GetTypes(implementedInterface, ass, alreadyLoaded))
                {
                    yield return type;
                }
            }

        }

        public virtual object ExecuteDynamicMethod(MethodInfo info, object def, out bool error, params object[] defaultparameters)
        {
            try
            {
                error = false;
                if (info == null)
                    return null;

                var pars = info.GetParameters();

                if (pars.Length <= 0)
                    return ExecuteMethod(info, def, out error);

                var args = new object[pars.Length];

                for (var i = 0; i < pars.Length; i++)
                {
                    var par = pars[i];

                    var pt = par.ParameterType;

                    var fit = defaultparameters.FirstOrDefault(t => t != null && pt.IsAssignableFrom(t.GetType()));

                    if (fit != null)
                    {
                        args[i] = fit;
                        continue;
                    }

                    var next = GetInstance(pt);
                    if (next != null)
                    {
                        args[i] = next;
                        continue;
                    }

                    args[i] = pt.IsValueType ? Activator.CreateInstance(pt) : null;
                }

                return ExecuteMethod(info, def, out error, args);
            }
            catch (Exception ex)
            {
                error = true;
                return ex;
            }
        }

        public virtual object ExecuteMethod(MethodInfo info, object def, out bool error, params object[] pars)
        {
            try
            {
                error = false;
                return info.Invoke(def, pars);
            }
            catch (Exception ex)
            {
                error = true;
                return ex;
            }
        }
    }
}
