angular.module('virtoCommerce.productRecommendationsModule')
.factory('virtoCommerce.productRecommendationsModule.recommendations', ['$resource', function ($resource) {

    return $resource(null, null, {
        exportCatalog: { method: 'GET', url: 'api/recommendations/stores/:storeId/catalog/export', params: { storeId: "@storeId" } },
        exportUsageEvents: { method: 'GET', url: 'api/recommendations/stores/:storeId/events', params: { storeId: "@storeId" } }
    });
}]);