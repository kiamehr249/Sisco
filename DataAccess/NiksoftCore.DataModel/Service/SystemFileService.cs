using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface ISystemFileService : IDataService<SystemFile>
    {
    }

    public class SystemFileService : DataService<SystemFile>, ISystemFileService
    {
        public SystemFileService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<SystemFile> GetPartOptional(List<Expression<Func<SystemFile, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.Id).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}