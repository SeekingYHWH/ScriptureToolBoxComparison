using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.IO;

namespace ScriptureToolBoxComparison
{
	internal static class Program
	{
		#region Main
		public static int Main(string[] args)
		{
			Prepare();
			return Run();
		}
		#endregion //Main

		#region Fields
		private static HttpClient client;
		private static Uri server = new Uri("https://scripturetoolbox.com/html/ic/");
		private static IDocument? document = new NullDocument();
		private static Book[] books = new Book[]
		{
			new Book() { Name = "Genesis", Chapters = new Chapter[]
			{
				new Chapter(01, "Genesis/1.html"),
				new Chapter(02, "Genesis/2.html"),
				new Chapter(03, "Genesis/3.html"),
				new Chapter(04, "Genesis/4.html"),
				new Chapter(05, "Genesis/5.html"),
				new Chapter(06, "Genesis/6.html"),
				new Chapter(07, "Genesis/7.html"),
				new Chapter(08, "Genesis/8.html"),
				new Chapter(09, "Genesis/9.html"),
				new Chapter(10, "Genesis/10.html"),
				new Chapter(11, "Genesis/11.html"),
				new Chapter(12, "Genesis/12.html"),
				new Chapter(13, "Genesis/13.html"),
				new Chapter(14, "Genesis/14.html"),
				new Chapter(15, "Genesis/15.html"),
				new Chapter(16, "Genesis/16.html"),
				new Chapter(17, "Genesis/17.html"),
				new Chapter(18, "Genesis/18.html"),
				new Chapter(19, "Genesis/19.html"),
				new Chapter(20, "Genesis/20.html"),
				new Chapter(21, "Genesis/21.html"),
				new Chapter(22, "Genesis/22.html"),
				new Chapter(23, "Genesis/23.html"),
				new Chapter(24, "Genesis/24.html"),
				new Chapter(25, "Genesis/25.html"),
				new Chapter(26, "Genesis/26.html"),
				new Chapter(27, "Genesis/27.html"),
				new Chapter(28, "Genesis/28.html"),
				new Chapter(29, "Genesis/29.html"),
				new Chapter(30, "Genesis/30.html"),
				new Chapter(31, "Genesis/31.html"),
				new Chapter(32, "Genesis/32.html"),
				new Chapter(33, "Genesis/33.html"),
				new Chapter(34, "Genesis/34.html"),
				new Chapter(35, "Genesis/35.html"),
				new Chapter(36, "Genesis/36.html"),
				new Chapter(37, "Genesis/37.html"),
				new Chapter(38, "Genesis/38.html"),
				new Chapter(39, "Genesis/39.html"),
				new Chapter(40, "Genesis/40.html"),
				new Chapter(41, "Genesis/41.html"),
				new Chapter(42, "Genesis/42.html"),
				new Chapter(43, "Genesis/43.html"),
				new Chapter(44, "Genesis/44.html"),
				new Chapter(45, "Genesis/45.html"),
				new Chapter(46, "Genesis/46.html"),
				new Chapter(47, "Genesis/47.html"),
				new Chapter(48, "Genesis/48.html"),
				new Chapter(49, "Genesis/49.html"),
				new Chapter(50, "Genesis/50.html"),
			} },
			new Book() { Name = "Exodus", Chapters = new Chapter[]
			{
				new Chapter(01, "Exodus/1.html"),
				new Chapter(02, "Exodus/2.html"),
				new Chapter(03, "Exodus/3.html"),
				new Chapter(04, "Exodus/4.html"),
				new Chapter(05, "Exodus/5.html"),
				new Chapter(06, "Exodus/6.html"),
				new Chapter(07, "Exodus/7.html"),
				new Chapter(08, "Exodus/8.html"),
				new Chapter(09, "Exodus/9.html"),
				new Chapter(10, "Exodus/10.html"),
				new Chapter(11, "Exodus/11.html"),
				new Chapter(12, "Exodus/12.html"),
				new Chapter(13, "Exodus/13.html"),
				new Chapter(14, "Exodus/14.html"),
				new Chapter(15, "Exodus/15.html"),
				new Chapter(16, "Exodus/16.html"),
				new Chapter(17, "Exodus/17.html"),
				new Chapter(18, "Exodus/18.html"),
				new Chapter(19, "Exodus/19.html"),
				new Chapter(20, "Exodus/20.html"),
				new Chapter(21, "Exodus/21.html"),
				new Chapter(22, "Exodus/22.html"),
				new Chapter(23, "Exodus/23.html"),
				new Chapter(24, "Exodus/24.html"),
				new Chapter(25, "Exodus/25.html"),
				new Chapter(26, "Exodus/26.html"),
				new Chapter(27, "Exodus/27.html"),
				new Chapter(28, "Exodus/28.html"),
				new Chapter(29, "Exodus/29.html"),
				new Chapter(30, "Exodus/30.html"),
				new Chapter(31, "Exodus/31.html"),
				new Chapter(32, "Exodus/32.html"),
				new Chapter(33, "Exodus/33.html"),
				new Chapter(34, "Exodus/34.html"),
				new Chapter(35, "Exodus/35.html"),
				new Chapter(36, "Exodus/36.html"),
				new Chapter(37, "Exodus/37.html"),
				new Chapter(38, "Exodus/38.html"),
				new Chapter(39, "Exodus/39.html"),
				new Chapter(40, "Exodus/40.html"),
			} },
			new Book() { Name = "Leviticus", Chapters = new Chapter[]
			{
				new Chapter(01, "Leviticus/1.html"),
				new Chapter(02, "Leviticus/2.html"),
				new Chapter(03, "Leviticus/3.html"),
				new Chapter(04, "Leviticus/4.html"),
				new Chapter(05, "Leviticus/5.html"),
				new Chapter(06, "Leviticus/6.html"),
				new Chapter(07, "Leviticus/7.html"),
				new Chapter(08, "Leviticus/8.html"),
				new Chapter(09, "Leviticus/9.html"),
				new Chapter(10, "Leviticus/10.html"),
				new Chapter(11, "Leviticus/11.html"),
				new Chapter(12, "Leviticus/12.html"),
				new Chapter(13, "Leviticus/13.html"),
				new Chapter(14, "Leviticus/14.html"),
				new Chapter(15, "Leviticus/15.html"),
				new Chapter(16, "Leviticus/16.html"),
				new Chapter(17, "Leviticus/17.html"),
				new Chapter(18, "Leviticus/18.html"),
				new Chapter(19, "Leviticus/19.html"),
				new Chapter(20, "Leviticus/20.html"),
				new Chapter(21, "Leviticus/21.html"),
				new Chapter(22, "Leviticus/22.html"),
				new Chapter(23, "Leviticus/23.html"),
				new Chapter(24, "Leviticus/24.html"),
				new Chapter(25, "Leviticus/25.html"),
				new Chapter(26, "Leviticus/26.html"),
				new Chapter(27, "Leviticus/27.html"),
			} },
			new Book() { Name = "Numbers", Chapters = new Chapter[]
			{
				new Chapter(01, "Numbers/1.html"),
				new Chapter(02, "Numbers/2.html"),
				new Chapter(03, "Numbers/3.html"),
				new Chapter(04, "Numbers/4.html"),
				new Chapter(05, "Numbers/5.html"),
				new Chapter(06, "Numbers/6.html"),
				new Chapter(07, "Numbers/7.html"),
				new Chapter(08, "Numbers/8.html"),
				new Chapter(09, "Numbers/9.html"),
				new Chapter(10, "Numbers/10.html"),
				new Chapter(11, "Numbers/11.html"),
				new Chapter(12, "Numbers/12.html"),
				new Chapter(13, "Numbers/13.html"),
				new Chapter(14, "Numbers/14.html"),
				new Chapter(15, "Numbers/15.html"),
				new Chapter(16, "Numbers/16.html"),
				new Chapter(17, "Numbers/17.html"),
				new Chapter(18, "Numbers/18.html"),
				new Chapter(19, "Numbers/19.html"),
				new Chapter(20, "Numbers/20.html"),
				new Chapter(21, "Numbers/21.html"),
				new Chapter(22, "Numbers/22.html"),
				new Chapter(23, "Numbers/23.html"),
				new Chapter(24, "Numbers/24.html"),
				new Chapter(25, "Numbers/25.html"),
				new Chapter(26, "Numbers/26.html"),
				new Chapter(27, "Numbers/27.html"),
				new Chapter(28, "Numbers/28.html"),
				new Chapter(29, "Numbers/29.html"),
				new Chapter(30, "Numbers/30.html"),
				new Chapter(31, "Numbers/31.html"),
				new Chapter(32, "Numbers/32.html"),
				new Chapter(33, "Numbers/33.html"),
				new Chapter(34, "Numbers/34.html"),
				new Chapter(35, "Numbers/35.html"),
				new Chapter(36, "Numbers/36.html"),
			} },
			new Book() { Name = "Deuteronomy", Chapters = new Chapter[]
			{
				new Chapter(01, "Deuteronomy/1.html"),
				new Chapter(02, "Deuteronomy/2.html"),
				new Chapter(03, "Deuteronomy/3.html"),
				new Chapter(04, "Deuteronomy/4.html"),
				new Chapter(05, "Deuteronomy/5.html"),
				new Chapter(06, "Deuteronomy/6.html"),
				new Chapter(07, "Deuteronomy/7.html"),
				new Chapter(08, "Deuteronomy/8.html"),
				new Chapter(09, "Deuteronomy/9.html"),
				new Chapter(10, "Deuteronomy/10.html"),
				new Chapter(11, "Deuteronomy/11.html"),
				new Chapter(12, "Deuteronomy/12.html"),
				new Chapter(13, "Deuteronomy/13.html"),
				new Chapter(14, "Deuteronomy/14.html"),
				new Chapter(15, "Deuteronomy/15.html"),
				new Chapter(16, "Deuteronomy/16.html"),
				new Chapter(17, "Deuteronomy/17.html"),
				new Chapter(18, "Deuteronomy/18.html"),
				new Chapter(19, "Deuteronomy/19.html"),
				new Chapter(20, "Deuteronomy/20.html"),
				new Chapter(21, "Deuteronomy/21.html"),
				new Chapter(22, "Deuteronomy/22.html"),
				new Chapter(23, "Deuteronomy/23.html"),
				new Chapter(24, "Deuteronomy/24.html"),
				new Chapter(25, "Deuteronomy/25.html"),
				new Chapter(26, "Deuteronomy/26.html"),
				new Chapter(27, "Deuteronomy/27.html"),
				new Chapter(28, "Deuteronomy/28.html"),
				new Chapter(29, "Deuteronomy/29.html"),
				new Chapter(30, "Deuteronomy/30.html"),
				new Chapter(31, "Deuteronomy/31.html"),
				new Chapter(32, "Deuteronomy/32.html"),
				new Chapter(33, "Deuteronomy/33.html"),
				new Chapter(34, "Deuteronomy/34.html"),
			} },
		};

		private static readonly StringBuilder builder = new StringBuilder();
		private static Wrote wrote;
		#endregion //Fields

		#region Methods
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
				for (var b = 0; b < books.Length; ++b)
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
				return -1;
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
