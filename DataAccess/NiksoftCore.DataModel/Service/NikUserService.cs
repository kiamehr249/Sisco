using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface INikUserService : IDataService<User>
    {
    }

    public class NikUserService : DataService<User>, INikUserService
    {
        public NikUserService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<User> GetPartOptional(List<Expression<Func<User, bool>>> predicate, int startIndex, int pageSize)
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