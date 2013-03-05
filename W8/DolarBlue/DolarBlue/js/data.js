(function () {
	"use strict";
	
	var divisas = new WinJS.Binding.List();
	
	function getExchangeRates(progressCallback, endCallback) {
		if (progressCallback) {
			progressCallback();
		}
			
		return WinJS.xhr({ url: 'http://servicio.abhosting.com.ar/divisa', type: 'post' }).then(function (xhr) {
			var result = JSON.parse(xhr.responseText);
			if (result && result.Divisas) {
				result.Divisas.forEach(function (divisa) {
					// items.Divisas.Nombre
					// items.Divisas.ValorCompra
					// items.Divisas.ValorVenta
					// items.Divisas.Variacion
					// items.Divisas.Simbolo
					// items.Divisas.Actualizacion
					divisas.push(divisa);
				});
			}
		}).then(function() {
			if (endCallback) {
				endCallback();
			}
		});			
	}

	WinJS.Namespace.define("Data", {
		getExchangeRatesFromService: getExchangeRates,
		exchangeRates: divisas
	});

})();
