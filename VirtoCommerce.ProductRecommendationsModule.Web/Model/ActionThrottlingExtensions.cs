using System;
using System.Threading.Tasks;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Model
{
    public static class ActionThrottlingExtensions
    {
        public static Action Throttle(this Action action, TimeSpan throttle)
        {
            var throttling = false;
            return () =>
            {
                if (throttling)
                    return;
                action();
                throttling = true;
                Task.Delay(throttle).ContinueWith(x => throttling = false);
            };
        }
    }
}