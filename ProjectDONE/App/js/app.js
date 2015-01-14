var onsReady = false;
ons.ready(function () {
    //Put some loading screen here, you know we'll need it
    onsReady = true;
});

(function () {
    'use strict';
    var app = angular.module('app', ['onsen', 'ngMessages']);
    //TODO: put user's auth token in local storage
    app.controller('AppController', function ($scope, $projectDone, $http, $timeout) {

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
                                          root_navigator.pushPage('Dashboard');
                                          $projectDone.GetOwnerJobs($projectDone.LoggedInUser.owner.ID)
                                          .then(function (results) {
                                              $projectDone.LoggedInUser.owner.Jobs = results.data;

                                          });
                                      });
            }
            else {
                root_navigator.pushPage('Login');
            }
        }
        $timeout($scope.CheckLogin, 500)
    });

    app.controller('registerController', function ($scope, User, $projectDone, $http) {
        $scope.register = {};

        $scope.register.RegisterClick = function () {
            $scope.register.form.$setSubmitted();
            $scope.register.Submit();
        };

        $scope.register.Submit = function () {
            if (!$scope.register.form.$valid) return;
            root_navigator.pushPage('loginLoading', {
                onTransitionEnd: function () {

                    $projectDone.Register($scope.register.email, $scope.register.password)
                    .then(function (result) {
                        $projectDone.Login($scope.register.email, $scope.register.password)
                        .then(function (result) {
                            root_navigator.pushPage('Dashboard');
                        })
                        .catch(function () {
                            root_navigator.popPage();
                            ons.notification.alert({ message: 'An error has occurred!' });
                        });
                    })
                    .catch(function (error) {
                        root_navigator.popPage();
                        ons.notification.alert({message: 'An error has occurred!'});
                    });
                }
            });
        }
    });

    app.controller('loginController', function ($scope, User, $projectDone, $http) {

        $scope.Login = function () {
            root_navigator.pushPage('loginLoading', {
                onTransitionEnd: function () {

                    $projectDone.Login($scope.email, $scope.password)
                    .then(function (result) {
                        root_navigator.pushPage('Dashboard');
                    })
                    .catch(function () {
                        root_navigator.popPage();
                        ons.notification.alert({ message: 'An error has occurred!' });
                    });
                }
            });
        };

        $scope.GoToRegister = function () {
            root_navigator.pushPage('Register');
        }
    });

    app.controller('dashboardController', function ($scope, $projectDone) {
        $scope.jobs = []
        $scope.loadingJobs = false;
        $scope.CreateJob = function () {
            root_navigator.pushPage('CreateJob');
        };

        $scope.ListJobs = function () {
            $scope.loadingJobs = true;
            $projectDone.GetJobs()
                .then(function (results) {
                    $scope.loadingJobs = false;
                    $scope.jobs = results.data;
                });
        };

        $scope.SelectJob = function (job) {
            $projectDone.SelectedJob = job;
            root_navigator.pushPage('ReviewJob');
        };

        $scope.ListJobs();

    });

    app.controller('createJobController', function ($scope, $projectDone, Job, Address) {
        $scope.Job = new Job();
        $scope.Job.Address = new Address();
        $scope.uploadingImage = false;
        $scope.imageData = null;
        $scope.image = null;

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
                    $scope.Job = new Job();
                    $scope.Job.Address = new Address();
                    $scope.imageData = $scope.image = null;
                });
            })
        };
    });

    app.controller('ReviewJobController', function ($scope, $projectDone, Job, Bid) {
        $scope.job = {}
        $scope.bid = new Bid();
        $scope.isJobOwner = null;

        $scope.loadJob = function () {
            $scope.job = $projectDone.SelectedJob;
            $projectDone.GetJob($scope.job.ID)
            .then(function (results) {
                $scope.job = results.data;
                $scope.isJobOwner = $scope.job.Owner_ID == $projectDone.LoggedInUser.owner.ID;
            });
        };

        $scope.BidOnCurrentJob = function () {
            $projectDone.PlaceBid($projectDone.SelectedJob.ID, $scope.bid)
            .then(function (results) {
                $scope.bid = new Bid();
            });
        };

        $scope.loadJob();
    });

    app.factory('User', function () {
        function User(User) {
            if (!!User)
                this.set(User);
        };

        User.prototype = {
            set: function (owner) {
                angular.extend(this, owner);
            },
            Owner: {}
        }

        return User;
    });

    app.factory('Job', ['$http', function ($http) {
        function Job(job) {
            if (!!job)
                this.set(job);
        };

        Job.prototype = {
            set: function (job) {
                angular.extend(this, job);
            },
            CreatedOn: "",
            CreatedByUserID: "",
            ID: "",
            TransactionID: "",
            Owner: "",
            Title: "",
            PublicDescription: "",
            Latitude: "",
            Longitude: "",
            Demographics: "",
            PrivateDescription: "",
            AcceptedBid_id: "",
            Media: {},
            Dialog: [],
            Bids: [],
            Address: {},
            AddressNotes: "",
            Earliest: "",
            DoneBy: "",
            MaxPay: 0,
            Status: 0
        };
        return Job;
    }]);

    app.factory('Bid', ['$http', function ($http) {
        function Bid(Bid) {
            if (!!Bid)
                this.set(Bid);
        };

        Bid.prototype = {
            set: function (Bid) {
                angular.extend(this, Bid);
            },
            CreatedOn: "",
            CreatedByUserID: "",
            ID: "",
            TransactionID: "",
            Owner: "",
            Amount: "",
            Dialog: [],
            job: "",
            Status: 0
        };
        return Bid;
    }]);

    app.factory('Address', function () {
        function Address(Address) {
            if (!!Address)
                this.set(Address);
        };

        Address.prototype =
        {
            set: function (address) {
                angular.extend(this, address);
            },
            ID: null,
            Name: "",
            Line1: "",
            Line2: "",
            City: "",
            State: "",
            Zip: ""
        };

        return Address;
    });

    app.factory('Media', function () {
        function Media(Media) {
            if (!!Media)
                this.set(Media);
        };
        Media.prototype = {
            set: function (media) {
                angular.extend(this, media);
            },
            ID: null,
            MIME_TYPE: "",
            URL: "",
            Title: "",
            Meta: ""
        };

    });

    app.service('$projectDone', function ($http, Job, Bid, $q) {
        var self = this;
        //TODO: find a way to pass odata to queries that support it.
        //User 
        self.LoggedInUser = {};

        self.SelectedJob = {};

        self.Login = function (username, password) {
            var deferred = $q.defer();

            username = encodeURIComponent(username);
            password = encodeURIComponent(password);
            var request =
                {
                    method: 'POST',
                    url: '/Token',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    data: 'grant_type=password&username=' + username + '&password=' + password

                };


            $http(request)
            .then(function (result) {
                var authKey = result.data.access_token;
                window.localStorage['authKey'] = authKey;
                $http.defaults.headers.common.Authorization = 'Bearer ' + authKey;

                self.GetOwner()
                          .then(function (result) {
                              self.LoggedInUser.owner = result.data;

                              self.GetOwnerJobs(self.LoggedInUser.owner.ID)
                              .then(function (results) {
                                  self.LoggedInUser.owner.Jobs = results.data;
                                  deferred.resolve();
                              })
                              .catch(function () {
                                  deferred.reject();
                              });
                          })
                            .catch(function () {
                                deferred.reject();
                            });
            })
            .catch(function () {
                deferred.reject();
            });
            return deferred.promise;
        };
        self.Register = function (username, password) {
            return $http.post('/api/account/register', {
                Email: username,
                Password: password
            });
        };
        self.GetOwner = function () {
            return $http.get("/api/app/Owner");
        };

        //Job
        self.CreateJob = function (job) {
            return $http.post('/api/app/Job', job);
        };
        self.GetJob = function (jobId) {
            return $http.get('/api/app/Job/' + jobId);
        };
        self.GetOwnerJobs = function (ownerId) {
            //TODO, Add pagination
            return $http.get('/api/app/job?$select=Title, ID&$filter=Owner_ID eq ' + ownerId);
        };
        self.GetJobs = function () {
            //TODO, pagination and filtering
            return $http.get('/api/app/job');
        };

        //Bid
        self.PlaceBid = function (jobId, bid) {
            return $http.post('/api/app/Job/' + jobId + '/Bid', bid);
        };
        self.GetBid = function (bidID) {
            console.log("Not Implemented");
        };
        self.AcceptBid = function (bidID) {
            return $http.post('/api/app/Bid/' + bidID + '/Accept');
        };
        self.ConfirmBid = function (bidID) {
            return $http.post('/api/app/Bid/' + bidID + '/Confirm');
        };
        self.WithdrawlBid = function (bidID) {
            console.log("Not implmented");
        };


        //Media
        this.UploadImage = function (file) {
            var fd = new FormData();
            fd.append('file', file);
            return $http.post('/api/media/upload', fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
           
        }
    })

    app.directive('fileModel', ['$parse', function ($parse) {
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

})();

