using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace DolarBlue.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
            ItemsRofex = new ObservableCollection<ItemViewModel>();
            ItemsTasas = new ObservableCollection<ItemViewModel>();
            TiposConversion = new ObservableCollection<ConversionViewModel>();
            Conversiones = new ObservableCollection<ConversionViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public ObservableCollection<ItemViewModel> ItemsRofex { get; private set; }

        public ObservableCollection<ItemViewModel> ItemsTasas { get; private set; }

        public ObservableCollection<ConversionViewModel> TiposConversion { get; private set; }

        public ObservableCollection<ConversionViewModel> Conversiones { get; private set; }

        private string _valorConvertir;
        public string ValorConvertir
        {
            get
            {
                return _valorConvertir;
            }
            set
            {
                if (value != _valorConvertir)
                {
                    _valorConvertir = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get { return IsDataLoadedDivisa && IsDataLoadedRofex && IsDataLoadedTasas; }
        }


        public bool IsDataLoadedDivisa
        {
            get;
            private set;
        }
        public bool IsDataLoadedRofex
        {
            get;
            private set;
        }
        public bool IsDataLoadedTasas
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData(Collection<ItemViewModel> items)
        {
            Items.Clear();
            TiposConversion.Clear();
            Conversiones.Clear();

            var peso = new ConversionViewModel
            {
                Nombre = "Peso",
                ValorVenta = 1,
                ValorConvertido = 1,
                Simbolo = "$",
                ValorConvertir = 1,
            };
            TiposConversion.Add(peso);
            Conversiones.Add(peso.DeepClone());

            foreach (var itemViewModel in items)
            {
                Items.Add(itemViewModel);

                double venta;
                if (!double.TryParse(itemViewModel.ValorVenta.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out venta)) continue;

                var divisa = new ConversionViewModel
                {
                    Nombre = itemViewModel.Nombre,
                    ValorVenta = venta,
                    ValorConvertido = venta,
                    Simbolo = itemViewModel.Simbolo,
                    ValorConvertir = 1,
                };
                TiposConversion.Add(divisa);
                Conversiones.Add(divisa.DeepClone(peso.Simbolo));
            }
            IsDataLoadedDivisa = true;
        }

        public void LoadDataRofex(Collection<ItemViewModel> items)
        {
            ItemsRofex.Clear();

            foreach (var itemViewModel in items)
            {
                ItemsRofex.Add(itemViewModel);
            }
            IsDataLoadedRofex = true;
        }

        public void LoadDataTasas(Collection<ItemViewModel> items)
        {
            ItemsTasas.Clear();

            foreach (var itemViewModel in items)
            {
                ItemsTasas.Add(itemViewModel);
            }
            IsDataLoadedTasas = true;
        }

        /// <summary>
        /// Calcula el nuevo valor convertido de cada divisa
        /// </summary>
        public void SetValorConversion(double valorAConvertir, ConversionViewModel valorOrigen)
        {
            double valorMonedaSeleccionadaEnPesos = Conversiones.First(x => x.Nombre.Equals(valorOrigen.Nombre)).ValorVenta;

            foreach (var monedaDestino in Conversiones)
            {
                monedaDestino.ValorConvertido = valorAConvertir * valorMonedaSeleccionadaEnPesos / monedaDestino.ValorVenta;
                monedaDestino.Simbolo = valorOrigen.Simbolo;
                monedaDestino.ValorConvertir = valorAConvertir;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}