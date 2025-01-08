using MasterProject.Services;

namespace MasterProject.Tests
{
    public interface IErrorService : IService
    {
        void Error(string message);
    }

    public class ErrorService : BaseService, IErrorService
    {
        void IErrorService.Error(string message)
        {
            
        }
    }
}
