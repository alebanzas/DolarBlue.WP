(function () {
	"use strict";

	function isInternetAvailable() {
		var internetProfile = Windows.Networking.Connectivity.NetworkInformation.getInternetConnectionProfile();
		return internetProfile != null && internetProfile.getNetworkConnectivityLevel() == Windows.Networking.Connectivity.NetworkConnectivityLevel.internetAccess;
	}

	function showConnectionError() {
		var popup = Windows.UI.Popups.MessageDialog("Ha habido un error intentando acceder a los nuevos datos o no hay conexiones de red disponibles.\nPor favor asegúrese de contar con acceso de red y vuelva a abrir la aplicación.", "Sin conexión");
		popup.showAsync();
	}

	WinJS.Namespace.define("Utils", {
		isInternetAvailable: isInternetAvailable,
		showConnectionError: showConnectionError
	});

})();