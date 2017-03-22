angular.module('virtoCommerce.productRecommendationsModule')
.controller('virtoCommerce.productRecommendationsModule.storeRecommendationsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: 'recommendationsConfiguration',
            title: 'productRecommendationsModule.blades.recommendationsConfiguration.title',
            subtitle: 'productRecommendationsModule.blades.recommendationsConfiguration.subtitle',
            controller: 'virtoCommerce.productRecommendationsModule.recommendationsConfigurationController',
            template: 'Modules/$(VirtoCommerce.ProductRecommendations)/Scripts/blades/recommendations-configuration.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }
}]);