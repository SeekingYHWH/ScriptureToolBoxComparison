using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ScriptureToolBoxComparison
{
	internal sealed class WebSource : ISource
	{
		private readonly Uri server;
		private readonly HttpClient client;

		public WebSource(Uri server)
		{
			this.server = server;
			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			};
			this.client = new HttpClient(handler);
			client.BaseAddress = server;
			var headers = client.DefaultRequestHeaders;
			headers.Add("Host", server.Host);
			headers.Add("Connection", "Keep-Alive");
			headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
			headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
		}

		public Stream GetStream(Chapter value)
		{
			return client.GetStreamAsync(value.Source).Result;
		}
	}
}
