using System.IO;
using System.Text;
using System.Xml;
using BTB.Utilities;
using DolarBlue.BLL;
using Microsoft.WindowsAzure.StorageClient;

namespace DolarBlue.Services
{
	public class TileXmlGenerator
	{
		public void GenerateAndSaveXmlFromDivisa(DivisaViewModel divisa)
		{
			string nombre = divisa.Nombre.ToUrl();
			var fileName = string.Format("{0}.xml", nombre);
			var blobStorageType = GBellmann.Azure.Storage.AzureAccount.DefaultAccount().CreateCloudBlobClient();
			var container = blobStorageType.GetContainerReference("dolarblue");

			using (var destination = new MemoryStream())
			{
				var wSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.GetEncoding("ISO-8859-1") };
				var xw = XmlWriter.Create(destination, wSettings);

				GenerateXmlFile(xw, divisa);

				destination.Seek(0, SeekOrigin.Begin);
				var destBlobReference = container.GetBlobReference(fileName);
				destBlobReference.Properties.ContentType = "text/xml";
				destBlobReference.Properties.ContentEncoding = "ISO-8859-1";
				destBlobReference.UploadFromStream(destination);
			}
		}

		public void GenerateXmlFile(XmlWriter xw, DivisaViewModel divisa)
		{
			//<tile>
			//  <visual>
			//	<binding template="TileSquareText02">
			//	  <text id="1">Text Field 1</text>
			//	  <text id="2">Text Field 2</text>
			//	</binding>  
			//  </visual>
			//</tile>
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
			xw.WriteEndElement();

			xw.WriteStartElement("text");
			xw.WriteStartAttribute("id");
			xw.WriteString("3");
			xw.WriteEndAttribute();
			xw.WriteString(string.Format("Venta: {0}", divisa.ValorVenta));
			xw.WriteEndElement();

			xw.WriteStartElement("text");
			xw.WriteStartAttribute("id");
			xw.WriteString("4");
			xw.WriteEndAttribute();
			xw.WriteString(string.Format("Actualizado: {0}", divisa.Actualizacion));
			xw.WriteEndElement();


			xw.WriteEndElement(); // binding
			xw.WriteEndElement(); // visual
			xw.WriteEndElement(); // tile

			xw.WriteEndDocument();
			xw.Flush();
		}

	}
}
