using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Services
{
    public interface ISerializationService : IService
    {
        public string[] GetDirectoryListings();
        public void SaveObject<T>(string path, T toSave);
        public T GetObject<T>(string path);
    }

    public interface ITextSerialization : ISerializationService 
    {
        
    }

    public interface IUnityObjectSerializer : ISerializationService
    {
        public new void SaveObject<U>(string path, U toSave) where U: UnityEngine.Object;
        public new U GetObject<U>(string name) where U : UnityEngine.Object;
    }
}
