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
		private static HttpClient client;
		private static Uri server = new Uri("https://scripturetoolbox.com/html/ic/");
		private static IDocument? document = new NullDocument();
		private static readonly List<Book> books = new List<Book>();

		private static readonly StringBuilder builder = new StringBuilder();
		private static Wrote wrote;
		#endregion //Fields

		#region Methods
		private static List<string> ParseCommandLine(string[] args)
		{
			ParseLoadBooks(@"Torah.config");

			return null;
		}

		private static void ParseLoadBooks(string path)
		{
			var doc = new XmlDocument();
			doc.Load(path);
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
						using (var readerStream = client.GetStreamAsync(chapter.Source).Result)
						using (var reader = new StreamReader(readerStream))
						{
							ParseChapter(reader);
						}
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
			string? line;

			builder.Clear();
			wrote = Wrote.None;

			while (true)
			{
				//line
				line = reader.ReadLine();
				if (line == null)
				{
					return;
				}
				//p
				if (!line.Contains("<p onclick=") || line.Contains("Intro"))
				{
					continue;
				}
				//line
				line = reader.ReadLine();
				if (line == null)
				{
					return;
				}
				//<sub>...)<sub>
				var offset = ParseSub(line);
				if (offset < 0)
				{
					continue;
				}
				ParseP(line, offset);
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
				var spanClose = value.IndexOf("</span>", spanOpen);
				if (spanClose < 0 || spanClose >= last)
				{
					return last;
				}
				var mode = value[spanOpen + 13];
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

		private static void ParseP(string value, int offset)
		{
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
			//State
			switch (wrote)
			{
			default:
				wrote = Wrote.Delete;
				break;

			case Wrote.Delete:
				builder.Append(' ');
				break;

			case Wrote.Insert:
				document!.WriteInsert(builder.ToString());
				builder.Clear();
				document!.WriteNormal(" ");
				wrote = Wrote.Delete;
				break;

			case Wrote.Normal:
				document!.WriteNormal(builder.ToString());
				builder.Clear();
				document!.WriteNormal(" ");
				wrote = Wrote.Delete;
				break;
			}
			//Append
			builder.Append(value, offset, length);
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
			//State
			switch (wrote)
			{
			default:
				wrote = Wrote.Insert;
				break;

			case Wrote.Delete:
				document!.WriteDelete(builder.ToString());
				builder.Clear();
				document!.WriteNormal(" ");
				wrote = Wrote.Insert;
				break;

			case Wrote.Insert:
				builder.Append(' ');
				break;

			case Wrote.Normal:
				document!.WriteNormal(builder.ToString());
				builder.Clear();
				document!.WriteNormal(" ");
				wrote = Wrote.Insert;
				break;
			}
			//Append
			builder.Append(value, offset, length);
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
			//State
			switch (wrote)
			{
			default:
				wrote = Wrote.Normal;
				break;

			case Wrote.Delete:
				document!.WriteDelete(builder.ToString());
				builder.Clear();
				wrote = Wrote.Normal;
				builder.Append(' ');
				break;

			case Wrote.Insert:
				document!.WriteInsert(builder.ToString());
				builder.Clear();
				wrote = Wrote.Normal;
				builder.Append(' ');
				break;

			case Wrote.Normal:
				builder.Append(' ');
				break;
			}
			//Append
			builder.Append(value, offset, length);
		}
		#endregion //Methods
	}
}
