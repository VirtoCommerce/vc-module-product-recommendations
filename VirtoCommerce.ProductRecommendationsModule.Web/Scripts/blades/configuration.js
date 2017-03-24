angular.module('virtoCommerce.productRecommendationsModule')
.controller('virtoCommerce.productRecommendationsModule.configurationController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;
    blade.headIcon = 'fa fa-thumbs-up';
    blade.isLoading = false;

    blade.toolbarCommands = [
    {
        name: "productRecommendationsModule.blades.configuration.labels.export-catalog",
        icon: 'fa fa-upload',
        executeMethod: function () {
            var newBlade = {
                id: 'catalogPreparedForRecommendationsExport',
                title: 'productRecommendationsModule.blades.catalogExport.title',
                controller: 'virtoCommerce.productRecommendationsModule.exportController',
                template: 'Modules/$(VirtoCommerce.ProductRecommendations)/Scripts/blades/export.tpl.html',
                catalogId: blade.store.catalog
            };
            bladeNavigationService.showBlade(newBlade, blade);
        },
        canExecuteMethod: function () { return true; }
    }];
}]);