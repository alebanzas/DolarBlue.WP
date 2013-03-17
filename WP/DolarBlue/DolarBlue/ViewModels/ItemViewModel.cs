using System;
using System.ComponentModel;

namespace DolarBlue.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _nombre;
        public string Nombre
        {
            get
            {
                return _nombre;
            }
            set
            {
                if (value != _nombre)
                {
                    _nombre = value;
                    NotifyPropertyChanged("Nombre");
                }
            }
        }

        private string _simbolo;
        public string Simbolo
        {
            get
            {
                return _simbolo;
            }
            set
            {
                if (value != _simbolo)
                {
                    _simbolo = value;
                    NotifyPropertyChanged("Simbolo");
                }
            }
        }

        private string _valorVenta;
        public string ValorVenta
        {
            get
            {
                return _valorVenta;
            }
            set
            {
                if (value != _valorVenta)
                {
                    _valorVenta = value;
                    NotifyPropertyChanged("ValorVenta");
                }
            }
        }

        private string _compraVenta;
        public string CompraVenta
        {
            get
            {
                return _compraVenta;
            }
            set
            {
                if (value != _compraVenta)
                {
                    _compraVenta = value;
                    NotifyPropertyChanged("CompraVenta");
                }
            }
        }

        private string _variacion;
        public string Variacion
        {
            get
            {
                return _variacion;
            }
            set
            {
                if (value != _variacion)
                {
                    _variacion = value;
                    NotifyPropertyChanged("Variacion");
                }
            }
        }

        private string _actualizacion;
        public string Actualizacion
        {
            get
            {
                return _actualizacion;
            }
            set
            {
                if (value != _actualizacion)
                {
                    _actualizacion = value;
                    NotifyPropertyChanged("Actualizacion");
                }
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