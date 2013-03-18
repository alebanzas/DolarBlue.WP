using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using BTB.Utilities;
using DolarBlue.BLL;
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
			//<tile>
			//  <visual>
			//	<binding template="TileSquareText02">
			//	  <text id="1">Text Field 1</text>
			//	  <text id="2">Text Field 2</text>
			//	</binding>  
			//  </visual>
			//</tile>
			foreach (var divisa in divisas.Divisas)
			{
				var fileName = string.Format("{0}.xml", divisa.Nombre.ToUrl());
				var blobStorageType = GBellmann.Azure.Storage.AzureAccount.DefaultAccount().CreateCloudBlobClient();
				var container = blobStorageType.GetContainerReference("dolarblue");

				using (var destination = new MemoryStream())
				{
					var wSettings = new XmlWriterSettings {Indent = true, Encoding = Encoding.GetEncoding("ISO-8859-1")};
					var xw = XmlWriter.Create(destination, wSettings);

					xw.WriteStartDocument();

					xw.WriteStartElement("tile");
					xw.WriteStartElement("visual");
					xw.WriteStartElement("binding");
					xw.WriteStartAttribute("template");
					xw.WriteString("TileSquareText01");
					xw.WriteEndAttribute();

					xw.WriteStartElement("text");
					xw.WriteStartAttribute("id");
					xw.WriteString("1");
					xw.WriteEndAttribute();
					xw.WriteString(divisa.Nombre);
					xw.WriteEndElement();

					xw.WriteStartElement("text");
					xw.WriteStartAttribute("id");
					xw.WriteString("2");
					xw.WriteEndAttribute();
					xw.WriteString(string.Format("Compra: {0}", divisa.ValorCompra));
					xw.WriteString(divisa.Nombre);
					xw.WriteEndElement();

					xw.WriteStartElement("text");
					xw.WriteStartAttribute("id");
					xw.WriteString("3");
					xw.WriteEndAttribute();
					xw.WriteString(string.Format("Venta: {0}", divisa.ValorVenta));
					xw.WriteString(divisa.Nombre);
					xw.WriteEndElement();

					xw.WriteStartElement("text");
					xw.WriteStartAttribute("id");
					xw.WriteString("4");
					xw.WriteEndAttribute();
					xw.WriteString(string.Format("Actualizado: {0}", divisa.Actualizacion));
					xw.WriteString(divisa.Nombre);
					xw.WriteEndElement();


					xw.WriteEndElement(); // binding
					xw.WriteEndElement(); // visual
					xw.WriteEndElement(); // tile

					xw.WriteEndDocument();
					xw.Flush();

					destination.Seek(0, SeekOrigin.Begin);
					var destBlobReference = container.GetBlobReference(string.Format("{0}.xml", fileName));
					destBlobReference.Properties.ContentType = "text/xml";
					destBlobReference.Properties.ContentEncoding = "ISO-8859-1";
					destBlobReference.UploadFromStream(destination);
				}
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
