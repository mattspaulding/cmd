var onsReady = false;
ons.ready(function () {
    //Put some loading screen here, you know we'll need it
    onsReady = true;
});

'use strict';
var app = angular.module('app', ['onsen', 'ngMessages', 'ngRoute', 'appControllers', 'appDirectives', 'appServices']);

app.config(['$routeProvider',
function ($routeProvider) {
    $routeProvider.
    when('/Register', {
        templateUrl: 'App/Templates/Register.html',
        controller: 'registerController'
    }).
  when('/App', {
      templateUrl: 'App/Templates/App.html',
      controller: 'appController'
  }).
  when('/LoginLoading', {
      templateUrl: 'App/Templates/LoginLoading.html',
      controller: 'loginLoadingController'
  }).
        when('/Login', {
            templateUrl: 'App/Templates/Login.html',
            controller: 'loginController'
        }).
     when('/Register', {
         templateUrl: 'App/Templates/Register.html',
         controller: 'registerController'
     }).
when('/TermsOfService', {
    templateUrl: 'App/Templates/TermsOfService.html',
    controller: 'termsOfServiceController'
}).
when('/Dashboard', {
    templateUrl: 'App/Templates/Dashboard.html',
    controller: 'dashboardController'
}).
when('/SearchJobs', {
    templateUrl: 'App/Templates/SearchJobs.html',
    controller: 'searchJobsController'
}).
when('/CreateJob', {
    templateUrl: 'App/Templates/CreateJob.html',
    controller: 'createJobController'
}).
when('/ReviewJob/:jobID', {
    templateUrl: 'App/Templates/ReviewJob.html',
    controller: 'reviewJobController'
}).
when('/PlaceBid', {
    templateUrl: 'App/Templates/PlaceBid.html',
    controller: 'placeBidController'
}).
      otherwise({
          redirectTo: '/App'
      });
}]);



