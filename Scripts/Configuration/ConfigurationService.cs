using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Services;
using UnityEngine;

namespace RIT.RochesterLOS.Configuraion
{
    public class ConfigurationService : IConfigurationService
    {
        private const string filePath = "/Config/ConfigObject";
        IByteSerialization serialization;
        private Dictionary<string, object> configData;
        public ConfigurationService()
        {
            serialization = (IByteSerialization)ServiceLocator.GetService<IByteSerialization>();
            
            //serialization.SaveObject<Dictionary<string, object>>(filePath + "ConfigData");
            
        }

        public object GetConfigValue(string valuePath)
        {
            LazyCheck();
            object val;
            if(configData.TryGetValue(valuePath, out val))
            {
                Debug.Log($"CONFIG: {valuePath}: {val}");
                return val;
            }
            Debug.LogWarning($"CONFIG: {valuePath} not found");
            return null;
        }

        public void UpdateValue(string valuePath, object value)
        {
            LazyCheck();
            if(!configData.TryAdd(valuePath, value))
            {
                Debug.Log("Updating Config Value: {valuePath} from {configData[valuePath]} to {value}");
                configData[valuePath] = value;
            }

            serialization.SaveObject(filePath, configData);
        }

        private void LazyCheck()
        {
            if(configData == null)
            {
                configData = serialization.GetObject<Dictionary<string, object>>(filePath);
            }
        }
    }
}
