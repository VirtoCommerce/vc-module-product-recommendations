using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Repositories
{
    public class UserEventRepository : EFRepositoryBase, IUserEventRepository
    {
        public UserEventRepository()
        {
        }

        public UserEventRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEvent>().ToTable("UserEvent");
            modelBuilder.Entity<UserEvent>().HasKey(x => x.Id)
                        .Property(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }

        #region IUserEventRepository Members

        public IQueryable<UserEvent> UserEvents
        {
            get { return GetAsQueryable<UserEvent>(); }
        }

        public UserEvent[] GetUserEventsByIds(string[] userEventsIds)
        {
            return UserEvents.Where(x => userEventsIds.Contains(x.Id)).ToArray();
        }

        #endregion
    }
}
