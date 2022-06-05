using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface IProvinceService : IDataService<Province>
    {
    }

    public class ProvinceService : DataService<Province>, IProvinceService
    {
        public ProvinceService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Province> GetPartOptional(List<Expression<Func<Province, bool>>> predicate, int startIndex, int pageSize)
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