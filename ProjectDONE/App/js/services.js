'use strict';

/* Services */

var appServices = angular.module('appServices', []);

appServices.factory('User', function () {
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

appServices.factory('Job', ['$http', function ($http) {
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
        Media: { URL: '' },
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

appServices.factory('Bid', ['$http', function ($http) {
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

appServices.factory('Address', function () {
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

appServices.factory('Media', function () {
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

appServices.service('$projectDone', function ($http, Job, Bid, $q) {
    var self = this;

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
    self.getLoggedInUser = function () {
        return self.LoggedInUser;
    }

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
    self.FinishJob = function (jobId) {
        return $http.post('/api/app/Job/' + jobId + '/Finish/')
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

    //Stripe
    self.StripePayment = function (job, stripeCallback) {
        StripeCheckout.configure({
            image: 'App/images/icon-logo.png',
            key: 'pk_test_6UITNydd2WGnJ9LVxEx7RZNR',
            token: stripeCallback


        }).open({
            name: 'Project: Done!',
            description: job.Title,
            amount: job.AcceptedBid.Amount * 100,
            email: job.Owner.Name
        });
    }
    self.TakePayment = function (job, stripeToken) {
        return $http.post(
                     '/api/app/Job/' + job.ID + '/MakePayment',
                     JSON.stringify(stripeToken))
    }

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
