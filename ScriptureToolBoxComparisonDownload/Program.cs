using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	internal static class Program
	{
		#region Main
		public static int Main(string[] args)
		{
			var errors = ParseCommandLine(args);
			if (errors != null)
			{
				PrintHelp(errors);
				return -1;
			}
			Prepare();
			return Run();
		}
		#endregion //Main

		#region Fields
		private static Uri server = new Uri("https://scripturetoolbox.com/html/ic/");
		private static HttpClient client;
		private static readonly List<Book> books = new List<Book>();
		#endregion //Fields

		#region Methods
		private static List<string> ParseCommandLine(string[] args)
		{
			var name = "Bible";
			BooksFactory.LoadBooks(books, name + ".config");

			return null;
		}

		private static void PrintHelp(List<string> errors)
		{
			Console.WriteLine();
			Console.WriteLine("ScriptureToolBoxComparisonDownload.exe");
			Console.WriteLine(" Required");
			Console.WriteLine(" Optional");
			Console.WriteLine();

			if (errors != null && errors.Count > 0)
			{
				foreach (var error in errors)
				{
					Console.Error.WriteLine(error);
				}
				Console.WriteLine();
			}
		}

		private static void Prepare()
		{
			Console.Title = "ScriptureToolBoxComparisonDownload";

			PrepareClient();

			GC.Collect();
		}

		private static void PrepareClient()
		{
			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			};
			client = new HttpClient(handler);
			client.BaseAddress = server;
			var headers = client.DefaultRequestHeaders;
			headers.Add("Host", server.Host);
			headers.Add("Connection", "Keep-Alive");
			headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
			headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
		}

		private static int Run()
		{
			for (var b = 0; b < books.Count; ++b)
			{
				var book = books[b];
				Console.WriteLine(book.Name);
				var chapters = book.Chapters;
				for (var c = 0; c < chapters.Length; ++c)
				{
					var chapter = chapters[c];
					Console.Write('\t');
					Console.WriteLine(chapter.Name);
					Stream? reader = null;
					FileStream? writer = null;
					try
					{
						reader = client.GetStreamAsync(chapter.Source).Result;
						var writerPath = Path.Combine(book.Name, chapter.Name + Path.GetExtension(chapter.Source));
						writer = new FileStream(writerPath, FileMode.Create, FileAccess.Write, FileShare.Read);
						reader.CopyTo(writer);
					}
					catch (Exception exception)
					{
						Console.Error.WriteLine(exception);
					}
					finally
					{
						writer?.Dispose();
						reader?.Dispose();
					}
				}
			}
			return 0;
		}
		#endregion //Methods
	}
}
