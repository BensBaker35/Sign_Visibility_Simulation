using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Services
{
    public class ServiceLocator
    {
        private static Dictionary<System.Type, IService> registeredServices;
        static ServiceLocator()
        {
            registeredServices = new();
        }
#nullable enable
        public static IService? GetService<S>() where S : IService
        {
            IService service;
            if (registeredServices.TryGetValue(typeof(S), out service))
            {
                return service;
            }
            Debug.LogWarning("Failed to find service of type: " + nameof(S));
            return null;
        }
#nullable disable

        public static void RegisterService<S>(S service) where S : IService
        {
            if(!registeredServices.TryAdd(typeof(S), service))
            {   
                Debug.LogWarning("Currently a registered service for: " + nameof(S));
                registeredServices[typeof(S)] = service;
            }
        }
    }

    public interface IService
    {

    }
}
