﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace NiksoftCore.DataAccess
{
    public class NikDbContext : DbContext, IUnitOfWork
    {

        public NikDbContext() : base()
        {
        }

        public NikDbContext(string connectionString) : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public DatabaseFacade DataFace
        {
            get { return this.Database; }
        }

        public async Task<int> SaveChangeAsync()
        {
            return await base.SaveChangesAsync();
        }

        public int SaveChange()
        {
            return base.SaveChanges();
        }



    }
}
