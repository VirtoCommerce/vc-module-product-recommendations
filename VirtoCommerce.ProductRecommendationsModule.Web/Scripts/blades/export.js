angular.module('virtoCommerce.productRecommendationsModule')
.controller('virtoCommerce.productRecommendationsModule.exportController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.productRecommendationsModule.recommendations', function ($scope, bladeNavigationService, recommendations) {
    var blade = $scope.blade;
    blade.headIcon = 'fa fa-upload';
    blade.isLoading = false;
    
    $scope.$on("new-notification-event", function (event, notification) {
        if (blade.notification && notification.id == blade.notification.id) {
            angular.copy(notification, blade.notification);
            if (notification.errorCount > 0) {
                bladeNavigationService.setError('Export error', blade);
            }
        }
    });

    function initializeBlade() {
        if (!blade.notification) {
            blade.isLoading = true;

            var resource = blade.exportType == "catalog" ? recommendations.exportCatalog : blade.exportType == "usageData" ? recommendations.exportUsageEvents : null;
            resource({ storeId: blade.storeId },
                function(data) {
                    blade.notification = data;
                    blade.isLoading = false;
                },
                function(error) { bladeNavigationService.setError('Error ' + error.status, $scope.blade); });
        }
    }

    initializeBlade();
}]);