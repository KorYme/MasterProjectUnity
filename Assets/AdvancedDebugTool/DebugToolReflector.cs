using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugMethod
    {
        public string Title;
        public bool UseDebugContext;
        public MethodInfo Method;

        public void Invoke(object objToInvoke, object[] parameters)
        {
            Method.Invoke(objToInvoke, UseDebugContext ? parameters : null);
        }
    }
    
    public class DebugTypeDefinition
    {
        public DebugMethod[] Methods;
        public HashSet<object> Instances = new HashSet<object>();
    }
    
    public interface IDebugInfoProvider
    {
        IEnumerable<DebugTypeDefinition> GetDebugInfos();
    }
    
    public class DebugToolReflector : IDebugInfoProvider
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        
        private Dictionary<Type, DebugTypeDefinition> m_Definitions = new Dictionary<Type, DebugTypeDefinition>();
        
        IEnumerable<DebugTypeDefinition> IDebugInfoProvider.GetDebugInfos()
        {
            return m_Definitions.Values;
        }
        
        public bool AddObjectToMenu(object objectToDebug)
        {
            Type type = objectToDebug.GetType();
            if (!m_Definitions.TryGetValue(type, out DebugTypeDefinition debugTypeDefinition))
            {
                MethodInfo[] methods = type.GetMethods(BINDING_FLAGS);
                DebugMethodAttribute methodAttribute;
                List<DebugMethod> debugMethods = new List<DebugMethod>();
                foreach (MethodInfo method in methods)
                {
                    if ((methodAttribute = method.GetCustomAttribute<DebugMethodAttribute>()) == null)
                    {
                        continue;
                    }

                    ParameterInfo[] parameters = method.GetParameters();
                    switch (parameters.Length)
                    {
                        case 0:
                        {
                            debugMethods.Add(new DebugMethod()
                            {
                                Title = methodAttribute.MenuTitle,
                                UseDebugContext = false,
                                Method = method,
                            });
                            break;
                        }
                        case 1:
                        {
                            if (parameters[0].ParameterType != typeof(DebugContext)) continue;
                            
                            debugMethods.Add(new DebugMethod()
                            {
                                Title = methodAttribute.MenuTitle,
                                UseDebugContext = true,
                                Method = method,
                            });
                            break;
                        }
                        default:
                            continue;
                    }
                }

                if (methods.Length == 0)
                {
                    m_Definitions[type] = null;
                    return false;
                }
                
                m_Definitions[type] = debugTypeDefinition = new DebugTypeDefinition()
                {
                    Methods = debugMethods.ToArray(),
                    Instances = new HashSet<object>(),
                };
            }
            return m_Definitions[type] != null && debugTypeDefinition.Instances.Add(objectToDebug);
        }

        public bool RemoveObjectFromMenu(object objectToDebug)
        {
            return m_Definitions.TryGetValue(objectToDebug.GetType(), out DebugTypeDefinition debugTypeDefinition)
                && debugTypeDefinition != null && debugTypeDefinition.Instances.Remove(objectToDebug);
        }
    }
}