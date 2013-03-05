// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
	        if (Utils.isInternetAvailable()) {
	        	Data.getExchangeRatesFromService(this.startProgress, this.endProgress).then(function() {
	        		var listView = element.querySelector(".cotizacioneslist").winControl;
	        		var itemTemplate = element.querySelector(".itemtemplate");

	        		listView.itemDataSource = Data.exchangeRates.dataSource;
	        		listView.groupDataSource = null;
	        		listView.itemTemplate = itemTemplate;
	        	});

	        } else {
				Utils.showConnectionError();
	        }
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in viewState.
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
    });
})();
