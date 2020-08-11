using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnderMineControl.Utility
{
    using API;
    
    public class ResourceUtility : IResourceUtility
    {
        /// <summary>
        /// Read all bytes starting at current position and ending at the end of the stream.
        /// </summary>
        public byte[] ReadAllBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Get a file set as "Embedded Resource" from the assembly that is calling this code, or optionally from a specified assembly.
        /// The filename is matched to the end of the resource path, no need to give the full path.
        /// If 0 or more than 1 resources match the provided filename, an exception is thrown.
        /// For example if you have a file "ProjectRoot\Resources\icon.png" set as "Embedded Resource", you can use this to load it by
        /// doing <code>GetEmbeddedResource("icon.png"), assuming that no other embedded files have the same name.</code>
        /// </summary>
        public byte[] GetEmbeddedResource(string resourceFileName, Assembly containingAssembly = null)
        {
            if (containingAssembly == null)
                containingAssembly = Assembly.GetCallingAssembly();

            var resourceName = containingAssembly.GetManifestResourceNames().Single(str => str.EndsWith(resourceFileName));

            using (var stream = containingAssembly.GetManifestResourceStream(resourceName))
                return ReadAllBytes(stream ?? throw new InvalidOperationException($"The resource {resourceFileName} was not found"));
        }

        /// <summary>
        /// Wrapper that allows unity to render a texture from a byte array
        /// </summary>
        /// <param name="data">The textures data</param>
        /// <returns>The <see cref="Texture2D"/> of the given resource</returns>
        public Texture2D LoadTexture(byte[] data)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            // Around Unity 2018 the LoadImage and other export/import methods got moved from Texture2D to extension methods
            var loadMethod = typeof(Texture2D).GetMethod("LoadImage", new[] { typeof(byte[]) });
            if (loadMethod != null)
            {
                loadMethod.Invoke(tex, new object[] { data });
            }
            else
            {
                var converter = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                if (converter == null)
                    throw new ArgumentNullException(nameof(converter));

                var converterMethod = converter.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
                if (converterMethod == null)
                    throw new ArgumentNullException(nameof(converterMethod));

                converterMethod.Invoke(null, new object[] { tex, data });
            }

            return tex;
        }
    }
}
