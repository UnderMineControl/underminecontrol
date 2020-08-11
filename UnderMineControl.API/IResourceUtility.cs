using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnderMineControl.API
{
    public interface IResourceUtility
    {
        byte[] ReadAllBytes(Stream input);

        byte[] GetEmbeddedResource(string filename, Assembly containingAssembly = null);

        Texture2D LoadTexture(byte[] data);
    }
}
