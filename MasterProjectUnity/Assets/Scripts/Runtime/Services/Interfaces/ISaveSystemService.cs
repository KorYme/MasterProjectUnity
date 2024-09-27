using MasterProject.SaveSystem;

namespace MasterProject.Services
{
    public interface ISaveSystemService<T>
    {
        void Register(IDataSaveable<T> dataSaveable);

        void Unregister(IDataSaveable<T> dataSaveable);

        T InitializeData();

        void LoadData(bool isLoadForced);

        void SaveData();
    }
}
