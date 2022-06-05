using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface INikRoleService : IDataService<Role>
    {
    }

    public class NikRoleService : DataService<Role>, INikRoleService
    {
        public NikRoleService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Role> GetPartOptional(List<Expression<Func<Role, bool>>> predicate, int startIndex, int pageSize)
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