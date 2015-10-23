using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DolarBlue.ViewModels;
using Microsoft.Phone.Controls;

namespace DolarBlue
{
    public partial class Conversor : PhoneApplicationPage
    {
        public Conversor()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
            
            Origen.ItemsSource = App.ViewModel.TiposConversion;

            Convertir_OnClick(null, null);
        }

        private void Convertir_OnClick(object sender, RoutedEventArgs e)
        {
            double valorConvertir;
            if (double.TryParse(ValorConvertir.Text.ToString(CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out valorConvertir))
            {
                App.ViewModel.SetValorConversion(valorConvertir, (ConversionViewModel)Origen.SelectedItem);
            }
            else
            {
                this.Dispatcher.BeginInvoke(() => MessageBox.Show("Ingrese un número."));
            }
        }
    }
}