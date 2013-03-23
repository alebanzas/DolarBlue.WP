// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

	var appView = Windows.UI.ViewManagement.ApplicationView;
	var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
	var ui = WinJS.UI;
	var notifications = Windows.UI.Notifications;
	
    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
    	ready: function (element, options) {
    		var that = this;
	        if (Utils.isInternetAvailable()) {
	        	Data.getExchangeRatesFromService(this.startProgress, this.endProgress).then(function() {
	        		var listView = element.querySelector(".cotizacioneslist").winControl;
	        		var itemTemplate = element.querySelector(".itemtemplate");

	        		that._initializeLayout(listView, appView.value, itemTemplate);
	        	});

	        } else {
				Utils.showConnectionError();
	        }
    		
	        notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(true);

    		var urisToPoll = [
    			new Windows.Foundation.Uri("http://dolarblue.cloudapp.net/api/dolarblue/dolar"),
    			new Windows.Foundation.Uri("http://dolarblue.cloudapp.net/api/dolarblue/dolar-blue"),
    			new Windows.Foundation.Uri("http://dolarblue.cloudapp.net/api/dolarblue/dolar-turistico")
    		];
    		var recurrence = notifications.PeriodicUpdateRecurrence.hour;
    		
    		notifications.TileUpdateManager.createTileUpdaterForApplication().startPeriodicUpdateBatch(urisToPoll, recurrence);
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

        	var listView = element.querySelector(".cotizacioneslist").winControl;

            var itemTemplate = element.querySelector(".itemtemplate");

            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listView.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    };

                    listView.addEventListener("contentanimating", handler, false);
                    this._initializeLayout(listView, viewState, itemTemplate);
                }

            }
        },
        
        startProgress: function() {
        	var pr = document.createElement("progress");
        	var header = document.querySelector("header h1");
        	header.appendChild(pr);
        },
        
        endProgress: function() {
        	var progress = document.querySelector("header h1 progress");
        	progress.style.display = "none";
        },
        
    	// This function updates the ListView with new layouts
        _initializeLayout: function (listView, viewState, itemTemplate) {
        	/// <param name="listView" value="WinJS.UI.ListView.prototype" />

        	listView.itemDataSource = Data.exchangeRates.dataSource;
        	listView.groupDataSource = null;
        	listView.itemTemplate = itemTemplate;

        	if (viewState === appViewState.snapped) {
        		listView.layout = new ui.ListLayout();
        	} else {
        		listView.layout = new ui.GridLayout();
        	}
        }

    });
})();
