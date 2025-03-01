namespace MasterProject.SaveSystem
{
    public interface IDataSaveable<T>
    {
        /// <summary>
        /// Will be called at the loading of the game, allows you to get your data back from the save class
        /// </summary>
        /// <param name="gameData">Class containing all of your data</param>
        void LoadData(T gameData);

        /// <summary>
        /// Save the data inside the device
        /// </summary>
        /// <param name="gameData"></param>
        void SaveData(ref T gameData);
    }
}
