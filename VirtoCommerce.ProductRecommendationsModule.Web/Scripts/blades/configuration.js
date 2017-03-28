angular.module('virtoCommerce.productRecommendationsModule')
.controller('virtoCommerce.productRecommendationsModule.configurationController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;
    blade.headIcon = 'fa fa-thumbs-up';
    blade.isLoading = false;

    var exportBlade = {
        controller: 'virtoCommerce.productRecommendationsModule.exportController',
        template: 'Modules/$(VirtoCommerce.ProductRecommendations)/Scripts/blades/export.tpl.html'
    };
    var exportToolbarCommand = {
        icon: 'fa fa-upload',
        canExecuteMethod: function () { return true; }
    }
    blade.toolbarCommands = [
        angular.merge({}, exportToolbarCommand,
        {
            name: "productRecommendationsModule.blades.configuration.labels.export-catalog",
            executeMethod: function () {
                var newBlade = angular.extend({}, exportBlade, {
                    id: 'catalogPreparedForRecommendationsExport',
                    title: 'productRecommendationsModule.blades.catalogExport.title',
                    storeId: blade.store.id,
                    exportType: "catalog"
                });
                bladeNavigationService.showBlade(newBlade, blade);
            }
        }),
        angular.merge({}, exportToolbarCommand,
        {
            name: "productRecommendationsModule.blades.configuration.labels.export-usageData",
            executeMethod: function () {
                var newBlade = angular.extend({}, exportBlade,{
                    id: 'UsageEventsRelatedToStoreExport',
                    title: 'productRecommendationsModule.blades.usageDataExport.title',
                    storeId: blade.store.id,
                    exportType: "usageData"
                });
                bladeNavigationService.showBlade(newBlade, blade);
            }
        })
    ];
}]);