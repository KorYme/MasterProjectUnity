using System;
using UnityEngine;

namespace MasterProject.Channels
{
    [CreateAssetMenu(menuName = "Channels/Simple Channel", order = 0)]
    public class Channel : ScriptableObject
    {
        private event Action m_OnChannelRaised;

        public void RaiseEvent()
        {
            m_OnChannelRaised?.Invoke();
        }

        public void BindCallBack(Action callback)
        {
            m_OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action callback)
        {
            m_OnChannelRaised -= callback;
        }
    }

    public abstract class Channel<T> : ScriptableObject
    {
        private event Action<T> m_OnChannelRaised;

        public void RaiseEvent(T value)
        {
            m_OnChannelRaised?.Invoke(value);
        }

        public void BindCallBack(Action<T> callback)
        {
            m_OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action<T> callback)
        {
            m_OnChannelRaised -= callback;
        }
    }

    public abstract class Channel<T0, T1> : ScriptableObject
    {
        private event Action<T0, T1> m_OnChannelRaised;

        public void RaiseEvent(T0 value0, T1 value1)
        {
            m_OnChannelRaised?.Invoke(value0, value1);
        }

        public void BindCallBack(Action<T0, T1> callback)
        {
            m_OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action<T0, T1> callback)
        {
            m_OnChannelRaised -= callback;
        }
    }

    public abstract class Channel<T0, T1, T2> : ScriptableObject
    {
        private event Action<T0, T1, T2> m_OnChannelRaised;

        public void RaiseEvent(T0 value0, T1 value1, T2 value2)
        {
            m_OnChannelRaised?.Invoke(value0, value1, value2);
        }

        public void BindCallBack(Action<T0, T1, T2> callback)
        {
            m_OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action<T0, T1, T2> callback)
        {
            m_OnChannelRaised -= callback;
        }
    }

    public abstract class Channel<T0, T1, T2, T3> : ScriptableObject
    {
        private event Action<T0, T1, T2, T3> m_OnChannelRaised;

        public void RaiseEvent(T0 value0, T1 value1, T2 value2, T3 value3)
        {
            m_OnChannelRaised?.Invoke(value0, value1, value2, value3);
        }

        public void BindCallBack(Action<T0, T1, T2, T3> callback)
        {
            m_OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action<T0, T1, T2, T3> callback)
        {
            m_OnChannelRaised -= callback;
        }
    }

    public abstract class Channel<T0, T1, T2, T3, T4> : ScriptableObject
    {
        private event Action<T0, T1, T2, T3, T4> m_OnChannelRaised;

        public void RaiseEvent(T0 value0, T1 value1, T2 value2, T3 value3, T4 value4)
        {
            m_OnChannelRaised?.Invoke(value0, value1, value2, value3, value4);
        }

        public void BindCallBack(Action<T0, T1, T2, T3, T4> callback)
        {
            m_OnChannelRaised += callback;
        }

        public void UnbindCallBack(Action<T0, T1, T2, T3, T4> callback)
        {
            m_OnChannelRaised -= callback;
        }
    }
}
