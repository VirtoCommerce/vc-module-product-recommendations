var moduleName = 'virtoCommerce.productRecommendationsModule';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.run(['platformWebApp.widgetService', function (widgetService) {
    widgetService.registerWidget({
        controller: 'virtoCommerce.productRecommendationsModule.storeRecommendationsWidgetController',
        template: 'Modules/$(VirtoCommerce.ProductRecommendations)/Scripts/widgets/store-recommendations-widget.tpl.html'
    }, 'storeDetail');
}]);