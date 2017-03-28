using System.Data.Entity;
using System.Linq;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Repositories
{
    public class UsageEventRepository : EFRepositoryBase, IUsageEventRepository
    {
        public UsageEventRepository()
        {
        }

        public UsageEventRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsageEventEntity>().ToTable("UsageEvent");
            modelBuilder.Entity<UsageEventEntity>().HasKey(x => x.Id)
                        .Property(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }

        #region IUsageEventRepository Members

        public IQueryable<UsageEventEntity> UsageEvents
        {
            get { return GetAsQueryable<UsageEventEntity>(); }
        }

        #endregion
    }
}
