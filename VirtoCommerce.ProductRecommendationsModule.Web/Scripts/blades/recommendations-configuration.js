angular.module('virtoCommerce.productRecommendationsModule')
.controller('virtoCommerce.productRecommendationsModule.recommendationsConfigurationController', ['$scope', function ($scope) {
    var blade = $scope.blade;
    blade.headIcon = 'fa fa-thumbs-up';
    blade.isLoading = false;
}]);