using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NiksoftCore.DataModel
{
    public interface IContentFileService : IDataService<ContentFile>
    {
    }

    public class ContentFileService : DataService<ContentFile>, IContentFileService
    {
        public ContentFileService(ISysUnitOfWork uow) : base(uow)
        {
        }

        public override IList<ContentFile> GetPartOptional(List<Expression<Func<ContentFile, bool>>> predicate, int startIndex, int pageSize)
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