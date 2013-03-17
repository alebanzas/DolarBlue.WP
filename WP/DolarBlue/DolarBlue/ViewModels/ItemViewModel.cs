using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DolarBlue
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _nombre;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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

        private string _valorVenta;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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