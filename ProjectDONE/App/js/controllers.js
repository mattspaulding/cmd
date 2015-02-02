'use strict';

/* Controllers */

var appControllers = angular.module('appControllers', []);
//TODO: put user's auth token in local storage
appControllers.controller('appController', function ($scope, $projectDone, $http, $timeout, $location) {

    $scope.CheckLogin = function () {
        if (!onsReady) {
            $timeout($scope.CheckLogin, 500)
            return
        }
        var authKey = window.localStorage['authKey'];
        if (authKey) {
            $http.defaults.headers.common.Authorization = 'Bearer ' + authKey;
            $projectDone.GetOwner()
                                  .then(function (result) {
                                      $projectDone.LoggedInUser.owner = result.data;
                                  })
                                  .then(function () {
                                      $location.path('Dashboard');
                                      $projectDone.GetOwnerJobs($projectDone.LoggedInUser.owner.ID)
                                      .then(function (results) {
                                          $projectDone.LoggedInUser.owner.Jobs = results.data;

                                      });
                                  });
        }
        else {
            $location.path('Login');
        }
    }
    $timeout($scope.CheckLogin, 500)
    $scope.LoggedInUser = $projectDone.getLoggedInUser();
});

appControllers.controller('registerController', function ($scope, User, $projectDone, $http, $location) {
    $scope.register = {};

    $scope.register.GoToTermsOfService = function () {
        $location.path('TermsOfService');
    };
    $scope.register.RegisterClick = function () {
        if ($scope.register.termsOfService === false || $scope.register.termsOfService === undefined) {
            alert("You must agree to the terms of service.");
        }
        else {
            $scope.register.form.$setSubmitted();
            $scope.register.Submit();
        }
    };



    $scope.register.Submit = function () {
        if (!$scope.register.form.$valid) return;
        $location.path('loginLoading', {
            onTransitionEnd: function () {

                $projectDone.Register($scope.register.email, $scope.register.password)
                .then(function (result) {
                    $projectDone.Login($scope.register.email, $scope.register.password)
                    .then(function (result) {
                        $location.path('Dashboard');
                    })
                    .catch(function () {
                        root_navigator.popPage();
                        ons.notification.alert({ message: 'An error has occurred!' });
                    });
                })
                .catch(function (error) {
                    root_navigator.popPage();
                    ons.notification.alert({ message: 'An error has occurred!' });
                });
            }
        });
    }
});

appControllers.controller('termsOfServiceController', function ($scope, User, $projectDone, $http, $location) {
    // Intentionally left empty
});
appControllers.controller('loginLoadingController', function ($scope, User, $projectDone, $http, $location) {
    // Intentionally left empty
});

appControllers.controller('loginController', function ($scope, User, $projectDone, $http, $location) {
    $scope.login = {};
    $scope.login.LoginClick = function () {
        $scope.login.form.$setSubmitted();
        $scope.login.Submit();
    };

    $scope.login.Submit = function () {
        $location.path('loginLoading');
        $projectDone.Login($scope.login.email, $scope.login.password)
        .then(function (result) {
            $location.path('Dashboard');
        })
        .catch(function () {
            ons.notification.alert({ message: 'An error has occurred!' });
        });
    }

    $scope.login.GoToRegister = function () {
        $location.path('Register');
    }
});

appControllers.controller('dashboardController', function ($scope, $projectDone, $location) {
    $scope.LoggedInUser = $projectDone.getLoggedInUser();

    $scope.CreateJob = function () {
        $location.path('CreateJob');
    };

    $scope.SearchJobs = function () {
        $location.path('SearchJobs');
    };

});

appControllers.controller('searchJobsController', function ($scope,$http, $projectDone, $location) {
    // root_navigator.on("prepop", function () {
    //     $scope.ListJobs();
    //});

    $scope.jobs = []
    $scope.loadingJobs = false;

    $scope.ListJobs = function () {
        var authKey = window.localStorage['authKey'];
        if (authKey) {
            $http.defaults.headers.common.Authorization = 'Bearer ' + authKey;
            $scope.loadingJobs = true;
            $projectDone.GetJobs()
                .then(function (results) {
                    $scope.loadingJobs = false;
                    $scope.jobs = results.data;
                });
        }
        else {
            $location.path('Login');
        }
    };

    $scope.SelectJob = function (job) {
        $projectDone.SelectedJob = job;
        $location.path('ReviewJob/' + job.ID);
    };

    $scope.ListJobs();

});

appControllers.controller('createJobController', function ($scope, $projectDone, $location, Job, Address) {
    $scope.Job = new Job();
    $scope.Job.Address = new Address();
    $scope.uploadingImage = false;
    $scope.imageData = null;
    $scope.image = '';

    $scope.$watch('imageData', function () {
        if ($scope.imageData) {
            var fr = new FileReader();
            fr.onload = function (e) {
                $scope.$apply(function () {
                    $scope.image = e.target.result;
                });
            };
            fr.readAsDataURL($scope.imageData);
        }

    });

    $scope.CreateJob = function () {
        $projectDone.UploadImage($scope.imageData)
        .then(function (results) {
            $scope.Job.Media = results.data;
            $projectDone.CreateJob($scope.Job)
            .then(function (results) {
                root_navigator.popPage();
            });
        })
    };
});

appControllers.controller('reviewJobController', function ($scope,$http, $projectDone, $location, $routeParams, Job, Bid) {
    $scope.job = undefined;
    $scope.bid = new Bid();
    $scope.isJobOwner = null;
    $scope.canPay = true;
    $scope.loadJob = function () {
        var authKey = window.localStorage['authKey'];
        if (authKey) {
            $http.defaults.headers.common.Authorization = 'Bearer ' + authKey;
            $projectDone.GetJob($routeParams.jobID)
            .then(function (results) {
                $scope.job = results.data;
                $scope.isJobOwner = $scope.job.Owner_ID == $projectDone.LoggedInUser.owner.ID;
            });
        }
        else
        {
            $location.path('Login');
        }
    };

    $scope.PlaceBid = function () {
        $location.path('PlaceBid');
    };

    $scope.AcceptBid = function (bid) {
        $projectDone.AcceptBid(bid.ID)
        .then(function (results) {
            console.log(results);
        });
    };

    $scope.ConfirmBid = function () {
        $projectDone.ConfirmBid($scope.job.AcceptedBid_ID)
        .then(function (results) {
            console.log(results);
        });
    };

    $scope.FinishJob = function () {
        $projectDone.FinishJob($scope.job.ID)
        .then(function (results) {
            console.log(results);
        });
    };

    $scope.PayJob = function () {
        $projectDone.StripePayment($scope.job,
            function (token) {

                $location.path('Processing', { animation: "fade" });
                $projectDone.TakePayment($scope.job, token)
                .then(function (results) {
                    root_navigator.popPage();
                    $scope.job.Status = 3;
                    $scope.canPay = false;
                })
                 .catch(function (error) {
                     root_navigator.popPage();
                     ons.notification.alert({ message: 'An error has occurred!' });
                 });
            });
    }

    $scope.loadJob();
});

appControllers.controller('placeBidController', function ($scope, $projectDone, $location, Job, Bid) {
    var job = $scope.job;
    var bid = $scope.bid;
    $scope.BidOnCurrentJob = function () {
        $projectDone.PlaceBid($projectDone.SelectedJob.ID, $scope.bid)
        .then(function (results) {
            $scope.bid = new Bid();
            ons.notification.alert({ title: 'Success', message: 'Your bid was submitted' });
        });
    };

    //TODO: Move feePercent to resource file
    var feePercent = .13;
    $scope.CalculateTotal = function () {
        $(".serviceCharge").slideDown("slow");
        var num = ($scope.bid.Amount.split('.')[1] || []).length;
        if (num > 2)
            $scope.bid.Amount = ($scope.bid.Amount * 1).toFixed(2);
        $scope.bid.ServiceCharge = ($scope.bid.Amount * feePercent).toFixed(2);
        $scope.bid.Total = ($scope.bid.Amount - $scope.bid.ServiceCharge).toFixed(2);
    };
    $scope.CalculateAmount = function () {
        $(".serviceCharge").slideDown("slow");
        var num = ($scope.bid.Total.split('.')[1] || []).length;
        if (num > 2)
            $scope.bid.Total = ($scope.bid.Total * 1).toFixed(2);
        $scope.bid.Amount = ($scope.bid.Total / (1 - feePercent)).toFixed(2);
        $scope.bid.ServiceCharge = ($scope.bid.Amount * feePercent).toFixed(2);
    };


});


