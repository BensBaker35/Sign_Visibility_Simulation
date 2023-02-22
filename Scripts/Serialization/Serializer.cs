using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RIT.RochesterLOS.Serialization
{
    public class Serializer
    {
        //Singleton Members
        private static Serializer instance;

        public static Serializer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new();
                }
                return instance;
            }
        }

        //Instance Members
        private Dictionary<System.Type, ISerializer> handlers;

        private Serializer()
        {
            handlers = new();
        }

        public void RegisterSerializationTarget<K>(string dataPath)
        {
            string fullPath = Application.persistentDataPath + dataPath;
            if (!Directory.Exists(fullPath))
            {
                Debug.Log("Creating Directory: " + fullPath);
                Directory.CreateDirectory(fullPath);
            }
            handlers.TryAdd(typeof(K), new SerializationHandler<K>(fullPath));
        }

        public void RegisterUnitySerializationTarget<K>(string dataPath) where K : UnityEngine.Object
        {
            handlers.TryAdd(typeof(K), new UnitySerializationHandler<K>(dataPath));
        }


        public K GetUnityObject<K>(string name) where K : UnityEngine.Object
        {
            ISerializer ser;
            if (handlers.TryGetValue(typeof(K), out ser))
            {
                return ((UnitySerializationHandler<K>)ser).GetObject(name);
            }
            Debug.LogWarning($"Failed to find Serializer registered for {nameof(K)}");
            return default(K);
        }

        public T GetObject<T>(string name)
        {
            ISerializer ser;
            if (handlers.TryGetValue(typeof(T), out ser))
            {
                return ((SerializationHandler<T>)ser).GetObject(name);
            }
            Debug.LogWarning($"Failed to find Serializer registered for {nameof(T)}");
            return default(T);
        }

        public void SaveObject<T>(T data, string name)
        {
            ISerializer ser;
            if (handlers.TryGetValue(typeof(T), out ser))
            {
                ((SerializationHandler<T>)ser).SaveObject(data, name);
            }
        }

        public string[] DirectoryListing(string path) 
        {
            var fullPath = Application.persistentDataPath + path;
            if(Directory.Exists(fullPath))
            {
                return Directory.GetFiles(fullPath);
            }
            else 
            {
                return new string[0];
            }
        }

        private interface ISerializer
        {
            public System.Type GetSerializingType();
        }

        private class SerializationHandler<T> : ISerializer
        {
            protected string dataPath;
            private BinaryFormatter bf;
            public SerializationHandler(string dataPath)
            {
                bf = new();
                if (!dataPath.EndsWith('/'))
                {
                    this.dataPath = dataPath + "/";
                }
                else
                {
                    this.dataPath = dataPath;
                }

                Debug.Log("Handler registerd for: " + dataPath);
            }

            public System.Type GetSerializingType()
            {
                return typeof(T);
            }

            public virtual T GetObject(string objectPath)
            {
                using (var f = File.OpenRead(dataPath + objectPath))
                {
                    return (T)bf.Deserialize(f);
                }
            }

            public virtual void SaveObject(T obj, string name)
            {
                using (var f = File.OpenWrite(dataPath + name))
                {
                    bf.Serialize(f, obj);
                }
            }

            private byte[] openAsBinary(string fullPath)
            {
                byte[] bytes;
                using (var f = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    bytes = new byte[f.Length];
                    f.Read(bytes, 0, (int)f.Length);

                }
                return bytes;
            }
        }

        private class UnitySerializationHandler<U> : SerializationHandler<U> where U : UnityEngine.Object
        {
            public UnitySerializationHandler(string dataPath) : base(dataPath)
            {

            }


            public override U GetObject(string objectName)
            {
                Debug.Log(dataPath + objectName);
                return Resources.Load<U>(dataPath + objectName);
            }
        }
    }
}
