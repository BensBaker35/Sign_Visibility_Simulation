using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RIT.RochesterLOS.Services;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace RIT.RochesterLOS.Serialization
{
    public class SerializationService : IByteSerialization
    {
        private string dataPath = Application.persistentDataPath;
        private BinaryFormatter bf;

        public SerializationService()
        {
            bf = new();
        }

        public string[] GetDirectoryListings(string path)
        {
            var fullPath = dataPath + path;
            if (Directory.Exists(fullPath))
            {
                return Directory.GetFiles(fullPath);
            }
            else
            {
                return new string[0];
            }
        }

        public T GetObject<T>(string path)
        {
            try
            {
                
                using (var f = File.Open(dataPath + path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    if(f.Length == 0)
                    {
                        return default(T);
                    }
                    return (T)bf.Deserialize(f);
                }
            }
            catch (DirectoryNotFoundException)
            {
                var dirEnd = path.LastIndexOf('/');
                var dirFromPath = path.Substring(0, dirEnd);
                Directory.CreateDirectory(dataPath + dirFromPath);

                return GetObject<T>(path);
            }
            catch (FileNotFoundException)
            {
                File.Create(dataPath + path);
                return GetObject<T>(path);
            }

        }

        public void SaveObject<T>(string path, T toSave)
        {


            try
            {
                using (var f = File.OpenWrite(dataPath + path))
                {
                    bf.Serialize(f, toSave);
                }
            }
            catch (DirectoryNotFoundException)
            {
                var dirEnd = path.LastIndexOf('/');
                var dirFromPath = path.Substring(0, dirEnd);
                Directory.CreateDirectory(dataPath + dirFromPath);

               SaveObject<T>(path, toSave);
            }
            catch (FileNotFoundException)
            {
                File.Create(dataPath + path);
                SaveObject<T>(path, toSave);
            }
        }
    }

    public class UnitySerializationService : IUnityObjectSerializer
    {
        public U GetUnityObject<U>(string name) where U : Object
        {
            return Resources.Load<U>(name);
        }
    }

    public class FileSerializationService : ITextSerialization
    {
        private string dataPath;

        public FileSerializationService()
        {
            dataPath = Application.persistentDataPath;
        }

        public string[] GetDirectoryListings(string path)
        {
            var fullPath = dataPath + path;
            if (Directory.Exists(fullPath))
            {
                return Directory.GetFiles(fullPath);
            }
            else
            {
                return new string[0];
            }
        }

        public IEnumerable<string> ReadLines(string path)
        {
            SerializationHelper.CheckFile(Application.persistentDataPath + path);

            return File.ReadAllLines(dataPath + path);
        }

        public Task<string[]> ReadLinesAsnyc(string path)
        {
            var dirSep = path.LastIndexOf('/');
            var dirPath = path.Substring(0, dirSep);
            SerializationHelper.CheckDirectory(Application.persistentDataPath + dirPath);
            SerializationHelper.CheckFile(Application.persistentDataPath + path);
            return File.ReadAllLinesAsync(Application.persistentDataPath + path, System.Text.Encoding.UTF8);
        }

        public void SaveLines(string path, IEnumerable<string> contents)
        {
            File.WriteAllLinesAsync(dataPath + path, contents, System.Text.Encoding.UTF8);
        }
    }
    internal static class SerializationHelper 
    {
        static SerializationHelper()
        {

        }

        public static void CheckDirectory(string path)
        {
            if(!Directory.Exists(path))
            {
                var d = Directory.CreateDirectory(path);
            }
        }

        public static void CheckFile(string path)
        {
            if(!File.Exists(path))
            {
                var f = File.Create(path);
                f.Close();
            }
        }
    }
}
