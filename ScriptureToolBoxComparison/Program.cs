﻿using System;
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
		private static IDocument document;
		private static IOrder order = new OriginalOrder();
		private static readonly List<Book> books = new List<Book>();
		#endregion //Fields

		#region Methods
		private static List<string> ParseCommandLine(string[] args)
		{
			document = new XMLLog(new XmlDocument().DocumentElement, "log.xml");
			ParseLoadBooks(@"Torah.config");

			return null;
		}

		private static void ParseLoadBooks(string configPath)
		{
			var doc = new XmlDocument();
			doc.Load(configPath);
			var config = doc.DocumentElement;
			var chapters = new List<Chapter>();
			foreach (XmlNode bookConfig in config.SelectNodes("Book"))
			{
				var bookName = bookConfig.Attributes["Name"].InnerText;
				foreach (XmlNode chapterConfig in bookConfig.SelectNodes("Chapter"))
				{
					var chapterName = chapterConfig.Attributes["Name"].InnerText;
					var chapterSource = chapterConfig.Attributes["Source"].InnerText;
					var chapter = new Chapter(chapterName, chapterSource);
					chapters.Add(chapter);
				}
				var bookChapters = chapters.ToArray();
				chapters.Clear();
				var book = new Book(bookName, bookChapters);
				books.Add(book);
			}
		}

		private static void PrintHelp(List<string> errors)
		{
			Console.WriteLine();
			Console.WriteLine("ScriptureToolBoxComparison.exe");
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
			Console.Title = "ScriptureToolBoxComparison";

			PrepareClient();
			PrepareOrder();

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

		private static void PrepareOrder()
		{
			order.SetDocument(document);
		}

		private static int Run()
		{
			try
			{
				for (var b = 0; b < books.Count; ++b)
				{
					var book = books[b];
					document.BookStart(book);
					var chapters = book.Chapters;
					for (var c = 0; c < chapters.Length; ++c)
					{
						var chapter = chapters[c];
						document.ChapterStart(chapter);
						order.ChapterStart(chapter);
						using (var readerStream = client.GetStreamAsync(chapter.Source).Result)
						using (var reader = new StreamReader(readerStream))
						{
							ParseChapter(reader);
						}
						order.ChapterFinish();
						document.ChapterFinish();
					}
					document.BookFinish();
				}
			}
			catch (Exception exception)
			{
				Console.Error.WriteLine(exception);
				return -2;
			}
			finally
			{
				document?.Dispose();
			}
			return 0;
		}

		private static void ParseChapter(StreamReader reader)
		{
			while (true)
			{
				//line
				var value = reader.ReadLine();
				if (value == null)
				{
					return;
				}
				//p
				if (!value.Contains("<p onclick=") || value.Contains("Intro"))
				{
					continue;
				}
				//line
				value = reader.ReadLine();
				if (value == null)
				{
					return;
				}
				//<sub>...)<sub>
				var offset = ParseSub(value);
				if (offset < 0)
				{
					continue;
				}
				while (true)
				{
					if (ParseP(value, offset))
					{
						break;
					}
				Read:
					value = reader.ReadLine();
					if (value == null)
					{
						return;
					}
					if (string.IsNullOrWhiteSpace(value))
					{
						goto Read;
					}
					offset = 0;
					while (char.IsWhiteSpace(value[offset]))
					{
						++offset;
					}
				}
			}
		}

		private static int ParseSub(string value)
		{
			//Data
			var subOpen = value.IndexOf("<sup>");
			if (subOpen < 0)
			{
				return -1;
			}
			subOpen += 5;
			var subClose = value.IndexOf("</sup>", subOpen);
			if (subClose < 0)
			{
				return -1;
			}
			var last = subClose + 6;
			var spanOpen = value.IndexOf("<span class=\"", subOpen);
			//Normal
			if (spanOpen < 0 || spanOpen >= last)
			{
				if (value[subClose - 1] == ')')
				{
					--subClose;
				}
				if (subOpen < subClose)
				{
					WriteNormal(value, subOpen, subClose - subOpen);
				}
				return last;
			}
			//Normal
			if (spanOpen > subOpen)
			{
				WriteNormal(value, subOpen, spanOpen - subOpen);
			}
			while (true)
			{
				//Data
				spanOpen += 13;
				var spanClose = value.IndexOf("</span>", spanOpen);
				if (spanClose < 0 || spanClose >= last)
				{
					return last;
				}
				var mode = value[spanOpen];
				spanOpen = value.IndexOf('>', spanOpen);
				if (spanOpen < 0 || spanOpen >= spanClose)
				{
					return last;
				}
				++spanOpen;
				subOpen = spanClose + 7;
				if (value[spanClose - 1] == ')')
				{
					--spanClose;
				}
				//Process
				if (spanOpen != spanClose)
				{
					switch (mode)
					{
					case 'd':
						WriteDelete(value, spanOpen, spanClose - spanOpen);
						break;

					case 'i':
						WriteInsert(value, spanOpen, spanClose - spanOpen);
						break;

					default:
						break;
					}
				}
				//Next
				spanOpen = value.IndexOf("<span class=\"", subOpen);
				if (spanOpen < 0 || spanOpen >= last)
				{
					//Normal
					if (subOpen < subClose)
					{
						if (value[subClose - 1] == ')')
						{
							--subClose;
						}
						if (subOpen < subClose)
						{
							WriteNormal(value, subOpen, subClose - subOpen);
						}
					}
					return last;
				}
			}
		}

		private static bool ParseP(string value, int open)
		{
			var pClose = value.IndexOf("</p>", open);
			bool done;
			int last;
			if (pClose > 0)
			{
				done = true;
				last = pClose;
			}
			else
			{
				done = false;
				last = value.Length;
			}
			while (true)
			{
				var spanOpen = value.IndexOf("<span class=\"", open);
				//Process
				if (spanOpen < 0 || spanOpen > last || open < spanOpen)
				{
					//Normal
					var close = spanOpen >= 0 ? Math.Min(spanOpen, last) : last;
					WriteNormal(value, open, close - open);
					open = close;
				}
				if (open == spanOpen)
				{
					//Data
					spanOpen += 13;
					var spanClose = value.IndexOf("</span>", spanOpen);
					if (spanClose < 0 || spanClose >= last)
					{
						return done;
					}
					var mode = value[spanOpen];
					spanOpen = value.IndexOf('>', spanOpen);
					if (spanOpen < 0 || spanOpen >= spanClose)
					{
						return done;
					}
					++spanOpen;
					open = spanClose + 7;
					if (value[spanClose - 1] == ')')
					{
						--spanClose;
					}
					//Process
					if (spanOpen != spanClose)
					{
						switch (mode)
						{
						case 'd':
							WriteDelete(value, spanOpen, spanClose - spanOpen);
							break;

						case 'i':
							WriteInsert(value, spanOpen, spanClose - spanOpen);
							break;

						default:
							break;
						}
					}
				}
				//Next
				if (open >= last)
				{
					return done;
				}
			}
		}

		private static void WriteDelete(string value, int offset, int length)
		{
			//Trim Right
			while (char.IsWhiteSpace(value[offset + length - 1]))
			{
				--length;
				if (length <= 0)
				{
					return;
				}
			}
			//Trim Left
			while (char.IsWhiteSpace(value[offset]))
			{
				++offset;
				--length;
				if (length <= 0)
				{
					return;
				}
			}
			//WriteDelete
			order.WriteDelete(value, offset, length);
		}

		private static void WriteInsert(string value, int offset, int length)
		{
			//Trim Right
			while (char.IsWhiteSpace(value[offset + length - 1]))
			{
				--length;
				if (length <= 0)
				{
					return;
				}
			}
			//Trim Left
			while (char.IsWhiteSpace(value[offset]))
			{
				++offset;
				--length;
				if (length <= 0)
				{
					return;
				}
			}
			//WriteInsert
			order.WriteInsert(value, offset, length);
		}

		private static void WriteNormal(string value, int offset, int length)
		{
			//Trim Right
			while (char.IsWhiteSpace(value[offset + length - 1]))
			{
				--length;
				if (length <= 0)
				{
					return;
				}
			}
			//Trim Left
			while (char.IsWhiteSpace(value[offset]))
			{
				++offset;
				--length;
				if (length <= 0)
				{
					return;
				}
			}
			//WriteNormal
			order.WriteNormal(value, offset, length);
		}
		#endregion //Methods
	}
}
