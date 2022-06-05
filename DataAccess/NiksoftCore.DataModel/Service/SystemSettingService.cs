using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface ISystemSettingService : IDataService<SystemSetting>
    {

    }


    public class SystemSettingService : DataService<SystemSetting>, ISystemSettingService
    {
        public SystemSettingService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<SystemSetting> GetPartOptional(List<Expression<Func<SystemSetting, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderByDescending(i => i.Id).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}