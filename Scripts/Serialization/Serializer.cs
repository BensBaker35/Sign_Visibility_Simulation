using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        }

        public void RegisterUnitySerializationTarget<K>(string dataPath) where K: UnityEngine.Object
        {
             handlers.TryAdd(typeof(K), new UnitySerializationHandler<K>(dataPath));
        }


        public K GetUnityObject<K>(string name) where K: UnityEngine.Object
        {
            ISerializer ser;
            if(handlers.TryGetValue(typeof(K), out ser))
            {
                return ((UnitySerializationHandler<K>)ser).GetObject(name);
            }
            Debug.LogWarning($"Failed to find Serializer registered for {nameof(K)}");
            return default(K);
        }

        private interface ISerializer 
        {
            public System.Type GetSerializingType();
        }

        private class SerializationHandler<T> : ISerializer
        {
            protected string dataPath;
            public SerializationHandler(string dataPath)
            {
                
                if(!dataPath.EndsWith('/'))
                {
                    this.dataPath = dataPath + "/";
                }
                else 
                {
                    this.dataPath = dataPath;
                }
            }

            public System.Type GetSerializingType()
            {
                return typeof(T);
            }

            public virtual T GetObject(string objectPath)
            {
                return default(T);
            }
        }

        private class UnitySerializationHandler<U> : SerializationHandler<U> where U: UnityEngine.Object
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
