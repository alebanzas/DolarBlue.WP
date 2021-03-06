﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using BTB.Utilities;
using Microsoft.WindowsAzure.StorageClient;

namespace DolarBlue.TileUpdate.Web.Controllers
{
    public class DolarBlueController : ApiController
    {
        //
        // GET: /DolarBlue/Oficial

        public HttpResponseMessage Get(string id)
        {
			var response = GenerateResponseMessage(id);

	        return response;
        }

	    private HttpResponseMessage GenerateResponseMessage(string tipoDolar)
	    {
			var blobStorageType = GBellmann.Azure.Storage.AzureAccount.DefaultAccount().CreateCloudBlobClient();
			var container = blobStorageType.GetContainerReference("dolarblue");

		    var response = new HttpResponseMessage();
		    try
		    {
				var xml = container.GetBlobReference(string.Format("{0}.xml", tipoDolar)).DownloadText();

			    response.StatusCode = HttpStatusCode.OK;
			    response.Content = new StringContent(xml);
			    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
				response.Content.Headers.Add("X-WNS-Expires", DateTime.UtcNow.AddHours(6).ToString("R"));
			    response.Content.Headers.Add("X-WNS-Tag", tipoDolar.Undasherize());
		    }
		    catch (Exception e)
		    {
			    response.StatusCode = HttpStatusCode.BadRequest;
			    response.Content = new StringContent(e.Message + "\r\n" + e.StackTrace);
			    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
		    }
		    return response;
	    }
    }
}
