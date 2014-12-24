var onsReady = false;
ons.ready(function () {
    //Put some loading screen here, you know we'll need it
    onsReady = true;
});

(function () {
    'use strict';
    var app = angular.module('app', ['onsen']);
    //TODO: put user's auth token in local storage
    app.controller('AppController', function ($scope, $projectDone, $http, $timeout) {

        $scope.CheckLogin = function () {
            if (!onsReady) {
                $timeout($scope.CheckLogin, 500)
                return
            }
            var authKey = window.localStorage['authKey'];
            if (authKey)
            {
                $http.defaults.headers.common.Authorization = 'Bearer ' + authKey;
                $projectDone.GetOwner()
                                      .then(function (result) {
                                          $projectDone.LoggedInUser.owner = result.data;
                                      })
                                      .then(function () {
                                          bottom_navigator.pushPage('Dashboard');
                                          $projectDone.GetOwnerJobs($projectDone.LoggedInUser.owner.ID)
                                          .then(function (results) {
                                              $projectDone.LoggedInUser.owner.Jobs = results.data;

                                          });
                                      });
            }
            else
            {
                bottom_navigator.pushPage('Login');
            }
        }
        $timeout($scope.CheckLogin, 500)
    });

    app.controller('loginController', function ($scope, User, $projectDone, $http) {

        $scope.Login = function () {
            bottom_navigator.pushPage('loginLoading', {
                onTransitionEnd: function () {

                    $projectDone.Login($scope.email, $scope.password)
                    .then(function (result) {
                        $scope.email = $scope.password = "";
                        var authKey = result.data.access_token;;
                        window.localStorage['authKey'] = authKey;
                        $http.defaults.headers.common.Authorization = 'Bearer ' + authKey;

                    })
                    .then(function () {
                        $projectDone.GetOwner()
                                  .then(function (result) {
                                      $projectDone.LoggedInUser.owner = result.data;
                                  })
                                  .then(function () {
                                      $projectDone.GetOwnerJobs($projectDone.LoggedInUser.owner.ID)
                                      .then(function (results) {
                                          $projectDone.LoggedInUser.owner.Jobs = results.data;
                                          bottom_navigator.pushPage('Dashboard');
                                      })
                                  });
                    });
                }
            });
        }
    });

    app.controller('dashboardController', function ($scope, $projectDone) {
        $scope.CreateJob = function ()
        {
            root_navigator.pushPage('CreateJob');
        }
        $scope.ListJobs = function () { }
    });

    app.controller('createJobController', function ($scope, $projectDone, Job, Address) {
        $scope.Job = new Job();
        $scope.Job.Address = new Address();
        $scope.CreateJob = function()
        {
            $projectDone.CreateJob($scope.Job)
            .then(function (results) {
                $scope.Job = new Job();
                console.log(results);
            });
        }
    });

    app.controller('DetailController', function ($scope) {

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
            Media: [],
            Dialog: [],
            Bids: [],
            Address: {},
            AddressNotes: "", 
            Earliest: "",
            DoneBy: "", 
            MaxPay:0,
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

    app.service('$projectDone', function ($http, Job, Bid) {
        //TODO: find a way to pass odata to queries that support it.
        //User 
        this.LoggedInUser = {};

        this.Login = function (username, password) {
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
            return $http(request);
        };
        this.Register = function (username, password, confirm) {
            return $http.post('/api/account/register', {
                Email: username,
                Password: password,
                ConfirmPassword: confirm
            });
        };
        this.GetOwner = function () {
            return $http.get("/api/app/Owner");
        };

        //Job
        this.CreateJob = function (job) {
            return $http.post('/api/app/Job', job);
        };
        this.GetJob = function (jobId) {
            return $http.get('/api/app/Job/' + jobId);
        };
        this.GetOwnerJobs = function (ownerId) {
            //TODO, Add pagination
            return $http.get('/api/app/job?$select=Title, ID&$filter=Owner_ID eq ' + ownerId);
        };

        //Bid
        this.PlaceBid = function (jobId, bid) {
            return $http.post('/api/app/Job/' + jobId + '/Bid', bid);
        };

        this.GetBid = function (bidID) {
            console.log("Not Implemented");
        };

        this.AcceptBid = function (bidID) {
            return $http.post('/api/app/Bid/' + bidID + '/Accept');
        };

        this.ConfirmBid = function (bidID) {
            return $http.post('/api/app/Bid/' + bidID + '/Confirm');
        };

        this.WithdrawlBid = function (bidID) {
            console.log("Not implmented");
        };


    })

})();
