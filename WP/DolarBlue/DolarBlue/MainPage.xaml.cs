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

namespace DolarBlue
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
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
            HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri("http://servicio.abhosting.com.ar/divisa"));
            httpReq.Method = "POST";
            httpReq.BeginGetResponse(HTTPWebRequestCallBack, httpReq);
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
            var result = new ObservableCollection<ItemViewModel>();
            
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
        }

        #endregion
    }
}