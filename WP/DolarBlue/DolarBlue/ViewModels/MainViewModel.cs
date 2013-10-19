﻿using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DolarBlue.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
            ItemsRofex = new ObservableCollection<ItemViewModel>();
            TiposConversion = new ObservableCollection<ConversionViewModel>();
            Conversiones = new ObservableCollection<ConversionViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public ObservableCollection<ItemViewModel> ItemsRofex { get; private set; }

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
            get { return IsDataLoadedDivisa && IsDataLoadedRofex; }
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
                if (!double.TryParse(itemViewModel.ValorVenta.Replace(',', '.').Split(' ')[1], NumberStyles.Any, CultureInfo.InvariantCulture, out venta)) continue;

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

        /// <summary>
        /// Calcula el nuevo valor convertido de cada divisa
        /// </summary>
        public void SetValorConversion(double valorConversion, ConversionViewModel valorOrigen)
        {
            foreach (var conversion in Conversiones)
            {
                conversion.ValorConvertido = valorConversion*conversion.ValorVenta / valorOrigen.ValorVenta;
                conversion.Simbolo = valorOrigen.Simbolo;
                conversion.ValorConvertir = valorConversion;
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