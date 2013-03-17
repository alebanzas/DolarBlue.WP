using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using DolarBlue.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace DolarBlue
{
    public partial class MainPage : PhoneApplicationPage
    {
        readonly ProgressIndicator _progress = new ProgressIndicator();
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            Loaded += MainPage_Loaded;

            _progress.IsVisible = true;
            _progress.IsIndeterminate = true;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                LoadData();
            }
        }


        #region DataInit


        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                ConnectionError.Visibility = Visibility.Collapsed;
                _progress.Text = "Buscando cotizaciones";
                SystemTray.SetIsVisible(this, true);
                SystemTray.SetProgressIndicator(this, _progress);

                var applicationBarIconButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
                if (applicationBarIconButton != null)
                    applicationBarIconButton.IsEnabled = false;

                var httpReq = (HttpWebRequest)WebRequest.Create(new Uri("http://servicio.abhosting.com.ar/divisa/?version=2"));
                httpReq.Method = "GET";
                httpReq.BeginGetResponse(HTTPWebRequestCallBack, httpReq);

                var httpReq2 = (HttpWebRequest)WebRequest.Create(new Uri("http://servicio.abhosting.com.ar/divisa/message/?version=2"));
                httpReq2.Method = "GET";
                httpReq2.BeginGetResponse(HTTPWebRequestMessageCallBack, httpReq);
            }
            else
            {
                ShowErrorConnection();
            }
        }

        private void HTTPWebRequestCallBack(IAsyncResult result)
        {
            try
            {
                var httpRequest = (HttpWebRequest)result.AsyncState;
                var response = httpRequest.EndGetResponse(result);
                var stream = response.GetResponseStream();

                var serializer = new DataContractJsonSerializer(typeof(DivisaModel));
                var o = (DivisaModel)serializer.ReadObject(stream);

                Dispatcher.BeginInvoke(new DelegateUpdateWebBrowser(UpdateCotizaciones), o);
            }
            catch (Exception)
            {
                EndRequest();
                //this.Dispatcher.BeginInvoke(() => MessageBox.Show("Error.. " + ex.Message));
                Dispatcher.BeginInvoke(() => MessageBox.Show("Ocurrió un error al obtener las cotizaciones. Verifique su conexión a internet."));
            }
        }

        private void HTTPWebRequestMessageCallBack(IAsyncResult result)
        {
            try
            {
                var httpRequest = (HttpWebRequest)result.AsyncState;
                var response = httpRequest.EndGetResponse(result);
                var stream = response.GetResponseStream();

                var serializer = new DataContractJsonSerializer(typeof(DivisaModel));
                var o = (MessageModel)serializer.ReadObject(stream);

                Dispatcher.BeginInvoke(() => MessageBox.Show(o.Message));
            }
            catch {}
        }

        delegate void DelegateUpdateWebBrowser(DivisaModel local);
        private void UpdateCotizaciones(DivisaModel model)
        {
            var result = new Collection<ItemViewModel>();
            
            foreach (var divisaViewModel in model.Divisas)
            {
                result.Add(new ItemViewModel
                {
                    Nombre = divisaViewModel.Nombre,
                    ValorVenta = string.Format("$ {0}", divisaViewModel.ValorVenta),
                    CompraVenta = string.Format("compra $ {0} | venta $ {1}",
                                                            divisaViewModel.ValorCompra,
                                                            divisaViewModel.ValorVenta),
                    Variacion = string.Format("variación: {0}", divisaViewModel.Variacion),
                    Actualizacion = string.Format("actualización: {0}", divisaViewModel.Actualizacion),
                    Simbolo = divisaViewModel.Simbolo,
                });
            }
            App.ViewModel.LoadData(result);
            EndRequest();
        }

        private void EndRequest()
        {
            var applicationBarIconButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (applicationBarIconButton != null)
                applicationBarIconButton.IsEnabled = true;
            Loading.Visibility = Visibility.Collapsed;
            SystemTray.SetProgressIndicator(this, null);
        }

        private void ShowErrorConnection()
        {
            //Luego le aviso al usuario que no se pudo cargar nueva información.
            ConnectionError.Visibility = Visibility.Visible;
            Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show("Ha habido un error intentando acceder a los nuevos datos o no hay conexiones de red disponibles.\nPor favor asegúrese de contar con acceso de red y vuelva a intentarlo."));
        }

        #endregion

        private void ButtonGo_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Conversor_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Conversor.xaml", UriKind.Relative));
        }
    }
}