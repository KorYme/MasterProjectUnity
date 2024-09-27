using MasterProject.Services;

namespace TLNTH
{
    public class TLNTHSaveSystemService : BaseSaveSystemService<TLNTHGameData>, ISaveSystemService<TLNTHGameData>
    {
        public override TLNTHGameData InitializeData()
        {
            return new TLNTHGameData()
            {

            }; 
        }
    }
}
