using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DolarBlue.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;

namespace DolarBlue
{
    public partial class MainPage : PhoneApplicationPage
    {
        ProgressIndicator progress = new ProgressIndicator();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            progress.IsVisible = true;
            progress.IsIndeterminate = true;
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
                progress.Text = "Buscando cotizaciones";
                SystemTray.SetIsVisible(this, true);
                SystemTray.SetProgressIndicator(this, progress);

                HttpWebRequest httpReq = (HttpWebRequest) HttpWebRequest.Create(new Uri("http://servicio.abhosting.com.ar/divisa"));
                httpReq.Method = "POST";
                httpReq.BeginGetResponse(HTTPWebRequestCallBack, httpReq);
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

                this.Dispatcher.BeginInvoke(new DelegateUpdateWebBrowser(UpdateWebBrowser), o);
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(() => MessageBox.Show("Error.. " + ex.Message));
            }
        }

        delegate void DelegateUpdateWebBrowser(DivisaModel local);
        private void UpdateWebBrowser(DivisaModel l)
        {
            DivisaModel model = l;
            var result = new Collection<ItemViewModel>();
            
            foreach (var divisaViewModel in model.Divisas)
            {
                result.Add(new ItemViewModel()
                {
                    LineOne = divisaViewModel.Nombre,
                    LineTwo = string.Format("$ {0}", divisaViewModel.ValorVenta),
                    LineThree = string.Format("compra $ {0} | venta $ {1}",
                                                            divisaViewModel.ValorCompra,
                                                            divisaViewModel.ValorVenta),
                    LineFour = string.Format("variación: {0}", divisaViewModel.Variacion),
                    LineFive = string.Format("actualización: {0}", divisaViewModel.Actualizacion),

                });
            }
            App.ViewModel.LoadData(result);
            EndRequest();
        }

        private void EndRequest()
        {
            SystemTray.SetProgressIndicator(this, null);
        }

        private void ShowErrorConnection()
        {
            //Luego le aviso al usuario que no se pudo cargar nueva información.
            ConnectionError.Visibility = Visibility.Visible;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("Ha habido un error intentando acceder a los nuevos datos o no hay conexiones de red disponibles.\nPor favor asegúrese de contar con acceso de red y vuelva a intentarlo.");
            });
        }

        #endregion

        private void ButtonGo_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}