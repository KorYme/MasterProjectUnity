using System;

namespace MasterProject.Utilities
{
    public class Flux<T> : IDisposable
    {
        public T Value;

        private Action<T> OnFluxUpdate;

        public Flux(T value)
        {
            OnFluxUpdate = null;
            Value = value;
        }

        public void OnUpdate()
        {
            OnFluxUpdate?.Invoke(Value);
        }

        public void Subscribe(Action<T> fluxUpdateCallback)
        {
            fluxUpdateCallback?.Invoke(Value);
            OnFluxUpdate += fluxUpdateCallback;
        }

        public void Unsubscribe(Action<T> fluxUpdateCallback)
        {
            OnFluxUpdate -= fluxUpdateCallback;
        }

        public void UnsubscribeAllCallbacks()
        {
            OnFluxUpdate = null;
        }

        void IDisposable.Dispose()
        {
            UnsubscribeAllCallbacks();
        }
    }
}
