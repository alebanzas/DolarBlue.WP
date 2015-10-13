using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using DolarBlue.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
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

            MobFoxAdControl.PublisherID = "336b241302471376ed5709debc76bac3";
            MobFoxAdControl.TestMode = false;

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
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ConnectionError.Visibility = Visibility.Collapsed;
                    ConnectionErrorRofex.Visibility = Visibility.Collapsed;
                    ConnectionErrorTasas.Visibility = Visibility.Collapsed;
                    _progress.Text = "Buscando cotizaciones";
                    SystemTray.SetIsVisible(this, true);
                    SystemTray.SetProgressIndicator(this, _progress);

                    var applicationBarIconButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
                    if (applicationBarIconButton != null)
                        applicationBarIconButton.IsEnabled = false;

                    Loading.Visibility = Visibility.Visible;
                    LoadingRofex.Visibility = Visibility.Visible;
                    LoadingTasas.Visibility = Visibility.Visible;
                    App.ViewModel.Items.Clear();
                    App.ViewModel.ItemsRofex.Clear();
                    App.ViewModel.ItemsTasas.Clear();
                });

                _requestCount = 2;
                var httpClient = new HttpClient();
                var httpReq = httpClient.Get(new Uri("http://servicio.abhosting.com.ar/api/cotizacion/divisas/?type=WP&version=2.1.0.0"));
                httpReq.BeginGetResponse(HTTPWebRequestCallBack, httpReq);

                LoadingRofex.Visibility = Visibility.Visible;
                var httpReqRofex = httpClient.Get(new Uri("http://servicio.abhosting.com.ar/api/cotizacion/rofex?type=WP&version=2.1.0.0"));
                httpReqRofex.BeginGetResponse(HTTPWebRequestRofexCallBack, httpReqRofex);

                LoadingTasas.Visibility = Visibility.Visible;
                var httpReqTasas = httpClient.Get(new Uri("http://servicio.abhosting.com.ar/api/cotizacion/tasas?type=WP&version=2.1.0.0"));
                httpReqTasas.BeginGetResponse(HTTPWebRequestTasasCallBack, httpReqTasas);
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
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ConnectionError.Visibility = Visibility.Visible;
                });
            }
        }

        private void HTTPWebRequestRofexCallBack(IAsyncResult result)
        {
            try
            {
                var httpRequest = (HttpWebRequest)result.AsyncState;
                var response = httpRequest.EndGetResponse(result);
                var stream = response.GetResponseStream();

                var serializer = new DataContractJsonSerializer(typeof(DivisaModel));
                var o = (DivisaModel)serializer.ReadObject(stream);

                Dispatcher.BeginInvoke(new DelegateUpdateRofexWebBrowser(UpdateCotizacionesRofex), o);
            }
            catch (Exception)
            {
                EndRequest();
                //this.Dispatcher.BeginInvoke(() => MessageBox.Show("Error.. " + ex.Message));
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ConnectionErrorRofex.Visibility = Visibility.Visible;
                });
            }
        }

        private void HTTPWebRequestTasasCallBack(IAsyncResult result)
        {
            try
            {
                var httpRequest = (HttpWebRequest)result.AsyncState;
                var response = httpRequest.EndGetResponse(result);
                var stream = response.GetResponseStream();

                var serializer = new DataContractJsonSerializer(typeof(DivisaModel));
                var o = (DivisaModel)serializer.ReadObject(stream);

                Dispatcher.BeginInvoke(new DelegateUpdateTasasWebBrowser(UpdateCotizacionesTasas), o);
            }
            catch (Exception)
            {
                EndRequest();
                //this.Dispatcher.BeginInvoke(() => MessageBox.Show("Error.. " + ex.Message));
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ConnectionErrorTasas.Visibility = Visibility.Visible;
                });
            }
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
            GenerateTile();

            Loading.Visibility = Visibility.Collapsed;
            EndRequest();
        }

        delegate void DelegateUpdateRofexWebBrowser(DivisaModel local);
        private void UpdateCotizacionesRofex(DivisaModel model)
        {
            var result = new Collection<ItemViewModel>();

            foreach (var divisaViewModel in model.Divisas)
            {
                result.Add(new ItemViewModel
                {
                    Nombre = divisaViewModel.Nombre,
                    ValorVenta = string.Format("$ {0}", divisaViewModel.ValorVenta),
                    //CompraVenta = string.Format("compra $ {0} | venta $ {1}",
                    //                                        divisaViewModel.ValorCompra,
                    //                                        divisaViewModel.ValorVenta),
                    Variacion = string.Format("variación: {0}", divisaViewModel.Variacion),
                    //Actualizacion = string.Format("actualización: {0}", divisaViewModel.Actualizacion),
                    Simbolo = divisaViewModel.Simbolo,
                });
            }
            App.ViewModel.LoadDataRofex(result);
            LoadingRofex.Visibility = Visibility.Collapsed;
            EndRequest();
        }

        delegate void DelegateUpdateTasasWebBrowser(DivisaModel local);
        private void UpdateCotizacionesTasas(DivisaModel model)
        {
            var result = new Collection<ItemViewModel>();

            foreach (var divisaViewModel in model.Divisas)
            {
                result.Add(new ItemViewModel
                {
                    Nombre = divisaViewModel.Nombre,
                    ValorVenta = string.Format("$ {0}", divisaViewModel.ValorVenta),
                    //CompraVenta = string.Format("compra $ {0} | venta $ {1}",
                    //                                        divisaViewModel.ValorCompra,
                    //                                        divisaViewModel.ValorVenta),
                    Variacion = string.Format("variación: {0}", divisaViewModel.Variacion),
                    //Actualizacion = string.Format("actualización: {0}", divisaViewModel.Actualizacion),
                    Simbolo = divisaViewModel.Simbolo,
                });
            }
            App.ViewModel.LoadDataTasas(result);
            LoadingTasas.Visibility = Visibility.Collapsed;
            EndRequest();
        }

        private int _requestCount;
        private void EndRequest()
        {
            _requestCount--;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (_requestCount == 0)
                    {
                        var applicationBarIconButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
                        if (applicationBarIconButton != null)
                            applicationBarIconButton.IsEnabled = true;

                        SystemTray.SetProgressIndicator(this, null);
                    }
                });
        }

        private void ShowErrorConnection()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ConnectionError.Visibility = Visibility.Visible;
                ConnectionErrorRofex.Visibility = Visibility.Visible;
                ConnectionErrorTasas.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Collapsed;
                LoadingRofex.Visibility = Visibility.Collapsed;
                LoadingTasas.Visibility = Visibility.Collapsed;
                MessageBox.Show("Ha habido un error intentando acceder a los nuevos datos o no hay conexiones de red disponibles.\nPor favor asegúrese de contar con acceso de red y vuelva a intentarlo.");
            });
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

        private void RateReview_Click(object sender, EventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void ButtonPin_Click(object sender, EventArgs e)
        {
            var name = "DolarBlueActualiza";

            var tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("TileID=1"));

            if (tileToFind != null) return;


            var periodicTask = new PeriodicTask(name)
            {
                Description = "Actualiza cotizacion del dolar en Tile",
            };
                
            if (ScheduledActionService.Find(name) != null)
            {
                ScheduledActionService.Remove(name);
            }
            ScheduledActionService.Add(periodicTask);

            if (GenerateTile()) return;

#if DEBUG
            ScheduledActionService.LaunchForTest(name, TimeSpan.FromSeconds(10));
#endif

        }

        private static bool GenerateTile()
        {
            if (!App.ViewModel.IsDataLoadedDivisa) return true;

            var item = App.ViewModel.Items.FirstOrDefault(x => x.Nombre.Contains("Blue"));

            if (item == null) return true;

            var newTileData = new StandardTileData
            {
                Title = item.Nombre,
                BackTitle = "DolarBlue",
                BackContent = item.ValorVenta,
            };


            ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("TileID=1"));

            if (tileToFind == null)
            {
                ShellTile.Create(new Uri("/MainPage.xaml?TileID=1", UriKind.Relative), newTileData);
            }
            else
            {
                tileToFind.Update(newTileData);
            }


            return false;
        }

        private void Opciones_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Opciones.xaml", UriKind.Relative));
        }
    }
}