using NiksoftCore.DataAccess;

namespace NiksoftCore.DataModel
{
    public interface IBaseInfoService : IDataService<BaseInfo>
    {
    }

    public class BaseInfoService : DataService<BaseInfo>, IBaseInfoService
    {
        public BaseInfoService(ISysUnitOfWork uow) : base(uow)
        {
        }
    }
}