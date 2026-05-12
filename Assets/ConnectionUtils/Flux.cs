using System;

namespace ConnectionUtils
{
    public class DisposableToken : IDisposable
    {
        private Action m_UnbindAction;

        public DisposableToken(Action unbindAction)
        {
            m_UnbindAction = unbindAction;
        }
        
        public void Dispose()
        {
            m_UnbindAction?.Invoke();
            m_UnbindAction = null;
        }
    }
    
    public class Flux<T> : IDisposable
    {
        private Action<T> m_Action;
        
        private T m_Value;
        
        public Flux(T value)
        {
            m_Value = value;
        }

        public void Notify(T newValue)
        {
            m_Value = newValue;
            m_Action?.Invoke(newValue);
        }
        
        public DisposableToken Bind(Action<T> action)
        {
            m_Action += action;
            action?.Invoke(m_Value);
            return new DisposableToken(() =>
            {
                m_Action -= action;
            });
        }

        public void Dispose()
        {
            m_Action = null;
        }
    }
}
