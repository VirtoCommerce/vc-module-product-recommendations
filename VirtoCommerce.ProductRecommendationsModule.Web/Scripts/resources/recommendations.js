angular.module('virtoCommerce.productRecommendationsModule')
.factory('virtoCommerce.productRecommendationsModule.recommendations', ['$resource', function ($resource) {

    return $resource(null, null, {
        exportCatalog: { method: 'GET', url: 'api/recommendations/catalogs/:catalogId/export', params: { catalogId: "@catalogId" } },
        exportUserEvents: { method: 'GET', url: 'api/recommendations/stores/:storeId/events', params: { storeId: "@storeId" } }
    });
}]);