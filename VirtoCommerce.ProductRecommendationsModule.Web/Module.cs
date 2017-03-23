using System.Linq;
using Microsoft.Practices.Unity;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ProductRecommendationsModule.Web
{
    public class Module : ModuleBase
    {

        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
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