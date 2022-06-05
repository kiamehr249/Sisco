using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface ICityService : IDataService<City>
    {
    }

    public class CityService : DataService<City>, ICityService
    {
        public CityService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<City> GetPartOptional(List<Expression<Func<City, bool>>> predicate, int startIndex, int pageSize)
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