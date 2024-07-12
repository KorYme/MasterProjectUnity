namespace MasterProject
{
    public interface IService
    {
        void Initialize();
        void Unload();
        void BaseUpdate(float deltaTime);
        void BaseLateUpdate(float deltaTime);
    }
}
