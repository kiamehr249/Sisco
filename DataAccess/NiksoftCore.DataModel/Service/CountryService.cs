using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface ICountryService : IDataService<Country>
    {
    }

    public class CountryService : DataService<Country>, ICountryService
    {
        public CountryService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<Country> GetPartOptional(List<Expression<Func<Country, bool>>> predicate, int startIndex, int pageSize)
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