using System;
using System.ComponentModel;
using System.Globalization;

namespace DolarBlue.ViewModels
{
    public class ConversionViewModel : INotifyPropertyChanged
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
                if (value == _nombre) return;

                _nombre = value;
                NotifyPropertyChanged("Nombre");
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
                if (value == _simbolo) return;

                _simbolo = value;
                NotifyPropertyChanged("Simbolo");
            }
        }

        private double _valorVenta;
        public double ValorVenta
        {
            get
            {
                return _valorVenta;
            }
            set
            {
                if (value.ToString(CultureInfo.InvariantCulture) == _valorVenta.ToString(CultureInfo.InvariantCulture)) return;

                _valorVenta = value;
                NotifyPropertyChanged("ValorVenta");
            }
        }


        private double _valorConvertir;
        public double ValorConvertir
        {
            get
            {
                return _valorConvertir;
            }
            set
            {
                if (value.ToString(CultureInfo.InvariantCulture) == _valorConvertir.ToString(CultureInfo.InvariantCulture)) return;

                _valorConvertir = value;
                NotifyPropertyChanged("ValorConvertir");
            }
        }


        public string ValorConvertidoStr
        {
            get { return ValorConvertido.ToString("##.###"); }
        }
        private double _valorConvertido;
        public double ValorConvertido
        {
            get
            {
                return _valorConvertido;
            }
            set
            {
                if (value.ToString(CultureInfo.InvariantCulture) == _valorConvertido.ToString(CultureInfo.InvariantCulture)) return;

                _valorConvertido = value;
                NotifyPropertyChanged("ValorConvertido");
                NotifyPropertyChanged("ValorConvertidoStr");
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

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Simbolo, Nombre);
        }

        public ConversionViewModel DeepClone()
        {
            return DeepClone(Simbolo);
        }

        public ConversionViewModel DeepClone(string simbolo)
        {
            return new ConversionViewModel
                {
                    Nombre = Nombre,
                    ValorConvertido = ValorConvertido,
                    Simbolo = simbolo,
                    ValorVenta = ValorVenta,
                    ValorConvertir = ValorConvertir,
                };
        }
    }
}