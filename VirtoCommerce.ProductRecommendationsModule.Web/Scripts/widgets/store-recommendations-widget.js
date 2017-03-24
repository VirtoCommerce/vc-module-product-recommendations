angular.module('virtoCommerce.productRecommendationsModule')
.controller('virtoCommerce.productRecommendationsModule.storeRecommendationsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: 'recommendationsConfiguration',
            title: 'productRecommendationsModule.blades.configuration.title',
            subtitle: 'productRecommendationsModule.blades.configuration.subtitle',
            controller: 'virtoCommerce.productRecommendationsModule.configurationController',
            template: 'Modules/$(VirtoCommerce.ProductRecommendations)/Scripts/blades/configuration.tpl.html',
            store: blade.currentEntity
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }
}]);