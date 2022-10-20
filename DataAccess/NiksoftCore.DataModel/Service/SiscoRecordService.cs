using NiksoftCore.DataAccess;

namespace NiksoftCore.DataModel
{
    public interface ISiscoRecordService : IDataService<SiscoRecord>
    {
    }
    public class SiscoRecordService : DataService<SiscoRecord>, ISiscoRecordService
    {
        public SiscoRecordService(ISysUnitOfWork uow) : base(uow)
        {
        }
    }
}