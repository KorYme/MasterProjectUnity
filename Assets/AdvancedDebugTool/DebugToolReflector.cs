using System;
using System.Collections.Generic;
using System.Reflection;

namespace AdvancedDebugTool
{
    public enum DebugCategory
    {
        General,
        Gameplay,
        Rendering,
        Other
    }
    
    public class DebugMethodInfo : IComparable<DebugMethodInfoInstance>
    {
        public string Title;
        public DebugCategory Category;
        public int Order;
        public MethodInfo Method;
        public bool UseDebugContext;

        public DebugMethodInfoInstance CreateMethodInstance(object instance)
        {
            return new DebugMethodInfoInstance()
            {
                Title = Title,
                Category = Category,
                Order = Order,
                Method = Method,
                UseDebugContext = UseDebugContext,
                Instance =  instance,
            };
        }

        public int CompareTo(DebugMethodInfoInstance other)
        {
            return other != null ? Order.CompareTo(other.Order) : -1;
        }
    }

    public class DebugMethodInfoInstance : DebugMethodInfo
    {
        public object Instance;

        public void Invoke(params object[] parameters)
        {
            Method.Invoke(Instance, UseDebugContext ? parameters : null);
        }
    }
    
    public class DebugTypeDefinition
    {
        public DebugMethodInfo[] Methods;
        public HashSet<object> Instances;
    }
    
    public interface IDebugInfoProvider
    {
        IReadOnlyList<DebugMethodInfoInstance> GetDebugInfos(DebugCategory category);
    }
    
    public class DebugToolReflector : IDebugInfoProvider
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        
        private Dictionary<Type, DebugTypeDefinition> m_TypeDefinitions;
        private Dictionary<DebugCategory, List<DebugMethodInfoInstance>> m_MethodInstances;

        public DebugToolReflector()
        {
            m_TypeDefinitions = new Dictionary<Type, DebugTypeDefinition>();
            
            m_MethodInstances = new Dictionary<DebugCategory, List<DebugMethodInfoInstance>>();
            foreach (DebugCategory category in Enum.GetValues(typeof(DebugCategory)))
            {
                m_MethodInstances.Add(category, new List<DebugMethodInfoInstance>());
            }
        }
        
        IReadOnlyList<DebugMethodInfoInstance> IDebugInfoProvider.GetDebugInfos(DebugCategory category)
        {
            return m_MethodInstances[category];
        }
        
        public bool AddObjectToMenu(object objectToDebug)
        {
            Type type = objectToDebug.GetType();
            if (!m_TypeDefinitions.TryGetValue(type, out DebugTypeDefinition debugTypeDefinition))
            {
                MethodInfo[] methods = type.GetMethods(BINDING_FLAGS);
                DebugMethodAttribute methodAttribute;
                List<DebugMethodInfo> debugMethods = new List<DebugMethodInfo>();
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
                            break;
                        case 1:
                        {
                            if (parameters[0].ParameterType != typeof(DebugContext)) continue;
                            break;
                        }
                        default:
                            continue;
                    }
                    debugMethods.Add(new DebugMethodInfo()
                    {
                        Title = methodAttribute.MenuTitle,
                        Category = methodAttribute.Category,
                        Order = methodAttribute.Order,
                        UseDebugContext = parameters.Length == 1,
                        Method = method,
                    });
                }

                if (methods.Length == 0)
                {
                    m_TypeDefinitions[type] = null;
                    return false;
                }
                
                m_TypeDefinitions[type] = debugTypeDefinition = new DebugTypeDefinition()
                {
                    Methods = debugMethods.ToArray(),
                    Instances = new HashSet<object>(),
                };
            }

            if (debugTypeDefinition == null)
            {
                // Means this type has already been registered but has no method with attribute
                return false;
            }

            if (debugTypeDefinition.Instances.Contains(objectToDebug))
            {
                // Means object is already registered
                return false;
            }
            
            debugTypeDefinition.Instances.Add(objectToDebug);
            foreach (DebugMethodInfo methodInfo in debugTypeDefinition.Methods)
            {
                m_MethodInstances[methodInfo.Category].Add(methodInfo.CreateMethodInstance(objectToDebug));
            }
            
            // TODO : Optimize here
            foreach ((DebugCategory _, List<DebugMethodInfoInstance> methodInstances) in m_MethodInstances)
            {
                methodInstances.Sort();
            }
            
            return true;
        }

        public bool RemoveObjectFromMenu(object objectToDebug)
        {
            if (!m_TypeDefinitions.TryGetValue(objectToDebug.GetType(), out DebugTypeDefinition debugTypeDefinition) || debugTypeDefinition == null)
            {
                return false;
            }

            foreach ((DebugCategory _, List<DebugMethodInfoInstance> methodInstances) in m_MethodInstances)
            {
                methodInstances.RemoveAll(item => item.Instance == objectToDebug);
            }

            return debugTypeDefinition.Instances.Remove(objectToDebug);
        }
    }
}