using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RIT.RochesterLOS.Services
{
    public interface ISerializationService : IService
    {
        public string[] GetDirectoryListings(string path);

    }

    public interface ITextSerialization : ISerializationService
    {
        public IEnumerable<string> ReadLines(string path);
        public Task<string[]> ReadLinesAsnyc(string path);
        public void SaveLines(string path, IEnumerable<string> contents);
    }

    public interface IByteSerialization : ISerializationService
    {
        public void SaveObject<T>(string path, T toSave);
        public T GetObject<T>(string path);
    }

    public interface IUnityObjectSerializer : IService
    {
        public  U GetUnityObject<U>(string name) where U : UnityEngine.Object;
    }
}
