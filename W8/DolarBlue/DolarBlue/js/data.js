(function () {
	"use strict";
	
	var divisas = new WinJS.Binding.List();
	
	function getExchangeRates(progressCallback, endCallback) {
		if (progressCallback) {
			progressCallback();
		}
			
		return WinJS.xhr({ url: 'http://servicio.abhosting.com.ar/divisa' }).then(function (xhr) {
			var result = JSON.parse(xhr.responseText);
			if (result && result.Divisas) {
				// result.Divisas.Nombre
				// result.Divisas.ValorCompra
				// result.Divisas.ValorVenta
				// result.Divisas.Variacion
				// result.Divisas.Simbolo
				// result.Divisas.Actualizacion
				result.Divisas.forEach(function (divisa) {
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
