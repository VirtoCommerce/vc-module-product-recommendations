using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations;
using VirtoCommerce.ProductRecommendationsModule.Data.Repositories;
using VirtoCommerce.ProductRecommendationsModule.Data.Services;

namespace VirtoCommerce.ProductRecommendationsModule.Web
{
    public class Module : ModuleBase
    {
        private const string ConnectionStringName = "VirtoCommerce";
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void SetupDatabase()
        {
            using (var context = new UsageEventRepository(ConnectionStringName, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<UsageEventRepository, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }
        }

        public override void Initialize()
        {
            _container.RegisterType<IUsageEventRepository>(new InjectionFactory(c => new UsageEventRepository(ConnectionStringName, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));
            _container.RegisterType<IUsageEventService, UsageEventService>();
            _container.RegisterType<IAzureRecommendationsClient, AzureRecommendationsClient>(new InjectionConstructor(ConfigurationManager.AppSettings["VirtoCommerce:ProductRecommendations:ApiKey"]));
            _container.RegisterType<IRecommendationsService, RecommendationsService>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            var settingManager = _container.Resolve<ISettingsManager>();
            var productRecommendationsSettings = settingManager.GetModuleSettings("VirtoCommerce.ProductRecommendations").ToArray();
            settingManager.RegisterModuleSettings("VirtoCommerce.Store", productRecommendationsSettings);
        }
    }
}