using System.Collections;
using System.Collections.Generic;
using RIT.RochesterLOS.Services;
using UnityEngine;

namespace RIT.RochesterLOS.Configuraion
{
    public class ConfigurationService : IConfigurationService
    {

        public ConfigurationService(string filePath)
        {
            
        }

        public object GetConfigValue(string valuePath)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateValue(string valuePath, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
