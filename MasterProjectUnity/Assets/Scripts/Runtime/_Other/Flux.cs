using System;

namespace MasterProject.Utilities
{
    public class ObjectFlux<T> : IDisposable
    {
        protected T m_Value;
        public virtual T Value
        {
            get => m_Value;
            set
            {
                if (!Equals(m_Value, value))
                {
                    m_Value = value;
                    OnFluxUpdate?.Invoke(value);
                }
            }
        }
        protected Action<T> OnFluxUpdate;
        protected ISubFluxUpdatable m_ContainingObject;

        public ObjectFlux(T value, ISubFluxUpdatable containingClass = null)
        {
            Value = value;
            m_ContainingObject = containingClass;
            OnFluxUpdate = ContainingClassUpdateCallback;
        }

        public void SetContainingObject(ISubFluxUpdatable newContainingClass)
        {
            m_ContainingObject = newContainingClass;
        }

        private void ContainingClassUpdateCallback(T _)
            => m_ContainingObject?.OnSubFluxUpdate?.Invoke();

        public void Subscribe(Action<T> fluxUpdateCallback)
        {
            fluxUpdateCallback?.Invoke(m_Value);
            OnFluxUpdate += fluxUpdateCallback;
        }

        public void Unsubscribe(Action<T> fluxUpdateCallback)
        {
            OnFluxUpdate -= fluxUpdateCallback;
        }

        public void UnsubscribeAllCallbacks(bool unsubContainingClass = false)
        {
            OnFluxUpdate = unsubContainingClass ? null : ContainingClassUpdateCallback;
        }

        public void Dispose()
        {
            UnsubscribeAllCallbacks(true);
        }
    }

    public class ClassFlux<T> : ObjectFlux<T> where T : ISubFluxUpdatable
    {
        public override T Value
        {
            get => m_Value;
            set
            {
                if (!Equals(m_Value, value))
                {
                    if (m_Value != null)
                    {
                        m_Value.OnSubFluxUpdate -= UpdateFluxValue;
                    }
                    m_Value = value;
                    if (m_Value != null)
                    {
                        m_Value.OnSubFluxUpdate += UpdateFluxValue;
                    }
                    OnFluxUpdate?.Invoke(value);
                }
            }
        }

        public ClassFlux(T value, ISubFluxUpdatable containingClass = null) : base(value, containingClass) { }

        private void UpdateFluxValue() => OnFluxUpdate?.Invoke(m_Value);
    }

    public interface ISubFluxUpdatable
    {
        public Action OnSubFluxUpdate { get; set; }
    }
}
