using System;
using UnityEngine;

namespace MasterProject.Channels
{
    [CreateAssetMenu(menuName = "Channels/Simple Channel", order = 0)]
    public class Channel : ScriptableObject
    {
        private event Action OnChannelRaised;

        public void RaiseEvent()
        {
            OnChannelRaised?.Invoke();
        }

        public void BindCallBack(Action callback)
        {
            OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action callback)
        {
            OnChannelRaised -= callback;
        }
    }

    //[CreateAssetMenu(menuName = "Channels/XXX Channel", order = 1)]
    public abstract class Channel<T> : ScriptableObject
    {
        private event Action<T> OnChannelRaised;

        public void RaiseEvent(T value)
        {
            OnChannelRaised?.Invoke(value);
        }

        public void BindCallBack(Action<T> callback)
        {
            OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action<T> callback)
        {
            OnChannelRaised -= callback;
        }
    }
}
