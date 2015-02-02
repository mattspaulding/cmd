'use strict';

/* Directives */

var appDirectives = angular.module('appDirectives', []);

appDirectives.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

appDirectives.directive('toolbar', function () {
    return {
        restrict: 'E',
        template: '<div class="menuBanner"><span class="left"><i class="fa fa-chevron-left fa-2x back-button"> {{back}}</i></span><span class="title">{{title}}</span></div>',
        scope: {
            back: '@back',
            title:'@title'
        },
        link: function (scope, element, attrs) {
            $(element[0]).on('click', function () {
                history.back();
                scope.$apply();
            });
        }
    }
})

