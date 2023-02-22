using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Services
{
    public interface IConfigurationService : IService
    {
        public object GetConfigValue(string valuePath);
        public void UpdateValue(string valuePath, object value);
    }
}
