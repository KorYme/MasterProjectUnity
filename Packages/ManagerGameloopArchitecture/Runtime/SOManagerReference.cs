using ManagerInjection.Managers;
using UnityEngine;

namespace ManagerInjection
{
    public interface IManagerReferenceSetter<T> where T : class, IManager
    {
        void SetManagerReference(T managerReference);

        void UnsetManagerReference();
    }
    
    public abstract class SOManagerReference<T> : 
        #if ODIN_INSPECTOR
        Sirenix.OdinInspector.SerializedScriptableObject, 
        #else
        ScriptableObject, 
        #endif
        IManagerReferenceSetter<T> where T : class, IManager
    {
        #if ODIN_INSPECTOR
        [field: Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly] 
        #endif
        public T Instance { get; protected set; }
    
        void IManagerReferenceSetter<T>.SetManagerReference(T managerReference)
        {
            if (Instance == null)
            {
                Instance = managerReference;
            }
            else
            {
                Debug.LogWarning($"Manager of type {typeof(T)} reference already set");
            }
        }
    
        void IManagerReferenceSetter<T>.UnsetManagerReference()
        {
            Instance = null;
        }

        
        public static SOManagerReference<T> GetManagerReferenceEditor()
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<SOManagerReference<T>>(GetSOManagerReferencePathEditor()); 
            #else
            return null;
            #endif
        }
        
        private static string GetSOManagerReferencePathEditor()
        {
            return $"Assets/_Project/Content/ManagersRef/{typeof(T).Name.Substring(1)}Ref.asset";
        }
    }
}
