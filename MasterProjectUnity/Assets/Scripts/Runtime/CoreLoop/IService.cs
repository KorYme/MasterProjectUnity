namespace MasterProject
{
    public interface IService
    {
        bool IsLoaded { get; }
        void Initialize();
        void Unload();
        void BaseUpdate(float deltaTime);
        void BaseLateUpdate(float deltaTime);
    }
}
