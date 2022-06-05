using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface INikUserRoleService : IDataService<UserRole>
    {
    }

    public class NikUserRoleService : DataService<UserRole>, INikUserRoleService
    {
        public NikUserRoleService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<UserRole> GetPartOptional(List<Expression<Func<UserRole, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.UserId).ThenBy(t => t.UserId).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}