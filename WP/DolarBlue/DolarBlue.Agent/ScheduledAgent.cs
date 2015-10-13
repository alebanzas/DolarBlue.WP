using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using DolarBlue.ViewModels;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace DolarBlueAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            if (task is PeriodicTask)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // Execute periodic task actions here.
                    ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("TileID=1"));
                    if (tileToFind == null)
                    {
                        NotifyComplete();
                        return;
                    }

                    //var httpClient = new HttpClient();
                    //var httpReq = httpClient.Get(new Uri("http://servicio.abhosting.com.ar/api/cotizacion/divisas/?type=WP&version=2.1.0.0"));
                    //httpReq.BeginGetResponse(HTTPWebRequestCallBack, httpReq);
                
                    var newTileData = new StandardTileData
                    {
                        Title = "Dólar Blue",
                        BackTitle = DateTime.UtcNow.ToShortTimeString(),
                        BackContent = "",
                    };

                    tileToFind?.Update(newTileData);

                    // If debugging is enabled, launch the agent again in one minute.
#if DEBUG
                    ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(10));
#endif

                });
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

                var dolarblue = o.Divisas.FirstOrDefault(x => x.Nombre.Contains("Blue"));

                if (dolarblue == null) return;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var newTileData = new StandardTileData
                    {
                        Title = "Dólar Blue",
                        BackTitle = DateTime.UtcNow.ToShortTimeString(),
                        BackContent = dolarblue.ValorVenta,
                    };

                    var tileToFind =
                        ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("TileID=1"));

                    tileToFind?.Update(newTileData);
                });
            }
            catch (Exception)
            {
                
            }
            finally
            {
                NotifyComplete();
            }
        }
    }
}