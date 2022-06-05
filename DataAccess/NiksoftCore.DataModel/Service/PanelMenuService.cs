using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface IPanelMenuService : IDataService<PanelMenu>
    {
    }

    public class PanelMenuService : DataService<PanelMenu>, IPanelMenuService
    {
        public PanelMenuService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<PanelMenu> GetPartOptional(List<Expression<Func<PanelMenu, bool>>> predicate, int startIndex, int pageSize)
        {
            var query = TEntity.Where(predicate[0]);
            for (int i = 1; i < predicate.Count; i++)
            {
                query = query.Where(predicate[i]);
            }
            return query.OrderBy(i => i.Ordering).ThenBy(t => t.Id).Skip(startIndex).Take(pageSize).ToList();
        }
    }
}