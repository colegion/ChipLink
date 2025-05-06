using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            if (Services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T)} is already registered. Overwriting!");
            }
            Services[typeof(T)] = service;
        }

        public static T Get<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }
        
            Debug.LogError($"Service of type {typeof(T)} is not registered!");
            return null;
        }

        public static void Unregister<T>() where T : class
        {
            if (!Services.Remove(typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T)} was not found to unregister.");
            }
        }
    }
}