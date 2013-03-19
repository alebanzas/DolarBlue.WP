using System;
using System.IO;
using System.Text;
using System.Xml;
using DolarBlue.BLL;
using DolarBlue.Services;
using NUnit.Framework;

namespace TileUpdateWorker.Tests
{
	public class GenerateXmlFileTests
	{
		[Test]
		public void WhenDivisaThenGenerateFile()
		{
			var divisa = new DivisaViewModel
				             {
					             Nombre = "Dolar Oficial",
					             Simbolo = "USD",
					             ValorCompra = "5,02",
					             ValorVenta = "5,08",
					             Actualizacion = "18/03/2013",
					             Variacion = "1,3%",
				             };
			var tileXmlGenerator = new TileXmlGenerator();

			const string fileName = "dolaroficial.xml";
			File.Delete(fileName);

			using (var destination = File.CreateText(fileName))
			{
				var wSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.GetEncoding("ISO-8859-1") };
				var xw = XmlWriter.Create(destination, wSettings);

				tileXmlGenerator.GenerateXmlFile(xw, divisa);

				//destination.Write(xw);
				
				destination.Flush();
			}

		}
	}
}
