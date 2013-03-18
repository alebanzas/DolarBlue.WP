using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using BTB.Utilities;
using DolarBlue.BLL;
using DolarBlue.Services;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace TileUpdateWorker
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			while (true)
			{
				var divisas = SendRequestGetResponse<DivisaModel>(new Uri("http://servicio.abhosting.com.ar/divisa"), () => new DivisaModel());
				CreateXmlFiles(divisas);
				Thread.Sleep(900000);
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}

		private void CreateXmlFiles(DivisaModel divisas)
		{
			var tileXmlGenerator = new TileXmlGenerator();
			foreach (var divisa in divisas.Divisas)
			{
				tileXmlGenerator.GenerateAndSaveXmlFromDivisa(divisa);
			}
		}


		private T SendRequestGetResponse<T>(Uri urlToSearch, Func<T> executeForNullOrError) where T : class
		{
			try
			{
				var request = WebRequest.Create(urlToSearch) as HttpWebRequest;
				string res;
				using (var response = request.GetResponse() as HttpWebResponse)
				{
					var reader = new StreamReader(response.GetResponseStream());

					res = reader.ReadToEnd();
				}

				var deserializedObject = JsonConvert.DeserializeObject<T>(res);
				return deserializedObject;
			}
			catch (Exception ex)
			{
				return executeForNullOrError();
			}
		}
	}
}
