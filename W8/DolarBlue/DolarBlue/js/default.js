// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;

    app.onactivated = function (args) {
    	if (args.detail.kind === activation.ActivationKind.launch) {

    		document.getElementById("refresh").onclick = refreshContent;

            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
	        
            if (app.sessionState.history) {
            	nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {
            	if (nav.location) {
            		nav.history.current.initialPlaceholder = true;
            		return nav.navigate(nav.location, nav.state);
            	} else {
            		return nav.navigate(Application.navigator.home);
            	}
            }));
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
    	// args.setPromise().
    	app.sessionState.history = nav.history;
    	args.setPromise();
    };
	
    app.onsettings = function (e) {
    	e.detail.applicationcommands = { "about": { title: "Acerca de", href: "/pages/about.html" } };
    	WinJS.UI.SettingsFlyout.populateSettings(e);
    };

    function refreshContent() {
    	var appbar = document.getElementById("appbar");

	    if (Data.exchangeRates && Data.exchangeRates.length != 0) {
		    Data.exchangeRates.length = 0;
	    }

    	Data.getExchangeRatesFromService(startProgress, endProgress);
    	appbar.winControl.hide();
    }
	
    function startProgress() {
    	var pr = document.querySelector("header h1 progress");
	    if (!pr) {
		    pr = document.createElement("progress");
		    var header = document.querySelector("header h1");
		    header.appendChild(pr);
	    } else {
		    pr.style.display = "block";
	    }

    }
        
    function endProgress() {
		var progress = document.querySelector("header h1 progress");
		progress.style.display = "none";
	}

    app.start();
})();
