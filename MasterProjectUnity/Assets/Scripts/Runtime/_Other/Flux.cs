using System;

namespace MasterProject.Utilities
{
    public class ObjectFlux<T> : IDisposable
    {
        protected T m_value;
        public virtual T Value
        {
            get => m_value;
            set
            {
                if (!Equals(m_value, value))
                {
                    m_value = value;
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
            fluxUpdateCallback?.Invoke(m_value);
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
            get => m_value;
            set
            {
                if (!Equals(m_value, value))
                {
                    if (m_value != null)
                    {
                        m_value.OnSubFluxUpdate -= UpdateFluxValue;
                    }
                    m_value = value;
                    if (m_value != null)
                    {
                        m_value.OnSubFluxUpdate += UpdateFluxValue;
                    }
                    OnFluxUpdate?.Invoke(value);
                }
            }
        }

        public ClassFlux(T value, ISubFluxUpdatable containingClass = null) : base(value, containingClass) { }

        private void UpdateFluxValue() => OnFluxUpdate?.Invoke(m_value);
    }

    public interface ISubFluxUpdatable
    {
        public Action OnSubFluxUpdate { get; set; }
    }
}
