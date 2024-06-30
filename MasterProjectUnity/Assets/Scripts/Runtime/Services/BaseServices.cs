namespace MasterProject.Services
{
    public abstract class BaseServices
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsEnabled { get; private set; } = false;

        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        public virtual void Enable()
        {
            if (IsEnabled) return;
            IsEnabled = true;
        }

        public virtual void Disable()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
        }

        public virtual void Update(float deltaTime)
        {
            if (!IsEnabled) return;
        }
    }
}