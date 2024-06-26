﻿using System;
using System.Collections.Generic;
using System.IO;
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
		private static IDocument document;
		private static IOrder order = new SortOrder();
		private static string pre;
		private static ISource source;
		private static readonly List<Book> books = new List<Book>();
		private static string post;
		#endregion //Fields

		#region Methods
		private static List<string> ParseCommandLine(string[] args)
		{
			var name = "Torah";
			document = new FreeSpireDocument(new XmlDocument().DocumentElement, name + ".docx");
				//new MultipleDocument(
				//new GemBoxDocument(new XmlDocument().DocumentElement, name + ".docx"),
				//new FreeSpireDocument(new XmlDocument().DocumentElement, name + ".docx"),
				//new HTMLDocument(new XmlDocument().DocumentElement, name + ".html"),
				//new TextLog(new XmlDocument().DocumentElement, name + ".log"),
				//new XMLLog(new XmlDocument().DocumentElement, name + ".xml"));
			pre = "Joseph.config";
			//source = new WebSource(new Uri("https://scripturetoolbox.com/html/ic/"));
			source = new FileSource(@"..\..\..\..\Source");
			BooksFactory.LoadBooks(books, name + ".config");

			return null;
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

			PrepareOrder();

			GC.Collect();
		}

		private static void PrepareOrder()
		{
			order.SetDocument(document);
		}

		private static int Run()
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(pre))
				{
					Console.WriteLine("[PRE]");
					XMLSource(pre);
				}
				Console.WriteLine("[SOURCE]");
				for (var b = 0; b < books.Count; ++b)
				{
					var book = books[b];
					Console.WriteLine(book.Name);
					document.BookStart(book);
					var chapters = book.Chapters;
					for (var c = 0; c < chapters.Length; ++c)
					{
						var chapter = chapters[c];
						Console.Write('\t');
						Console.WriteLine(chapter.Name);
						document.ChapterStart(chapter);
						order.ChapterStart(chapter);
						using (var readerStream = source.GetStream(chapter))
						using (var reader = new StreamReader(readerStream))
						{
							ParseChapter(reader);
						}
						order.ChapterFinish();
						document.ChapterFinish();
					}
					document.BookFinish();
				}
				if (!string.IsNullOrWhiteSpace(post))
				{
					Console.WriteLine("[POST]");
					XMLSource(post);
				}
			}
			catch (Exception exception)
			{
				Console.Error.WriteLine(exception);
				return -2;
			}
			finally
			{
				Console.WriteLine("[CLOSING]");
				document?.Dispose();
			}
			return 0;
		}

		private static void XMLSource(string path)
		{
			var doc = new XmlDocument();
			doc.Load(path);
			var config = doc.DocumentElement;
			foreach (XmlNode bookConfig in config.SelectNodes("Book"))
			{
				var bookName = bookConfig.Attributes["Name"].InnerText;
				Console.WriteLine(bookName);
				var book = new Book(bookName);
				document.BookStart(book);
				foreach (XmlNode chapterConfig in bookConfig.SelectNodes("Chapter"))
				{
					var chapterName = chapterConfig.Attributes["Name"].InnerText;
					Console.Write('\t');
					Console.WriteLine(chapterName);
					var chapter = new Chapter(chapterName);
					document.ChapterStart(chapter);
					order.ChapterStart(chapter);
					foreach (XmlNode node in chapterConfig.ChildNodes)
					{
						var value = node.InnerText;
						switch (node.Name)
						{
						case "Delete":
							WriteDelete(value, 0, value.Length);
							break;

						case "Insert":
							WriteInsert(value, 0, value.Length);
							break;

						case "Normal":
							WriteNormal(value, 0, value.Length);
							break;

						case "Barrier":
							order.Barrier();
							break;
						}
					}
					order.ChapterFinish();
					document.ChapterFinish();
				}
				document.BookFinish();
			}
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
					order.Barrier();
				}
				return last;
			}
			var barrier = false;
			//Normal
			if (spanOpen > subOpen)
			{
				WriteNormal(value, subOpen, spanOpen - subOpen);
				barrier = true;
			}
			while (true)
			{
				//Data
				spanOpen += 13;
				var spanClose = value.IndexOf("</span>", spanOpen);
				if (spanClose < 0 || spanClose >= last)
				{
					if (barrier)
					{
						order.Barrier();
					}
					return last;
				}
				var mode = value[spanOpen];
				spanOpen = value.IndexOf('>', spanOpen);
				if (spanOpen < 0 || spanOpen >= spanClose)
				{
					if (barrier)
					{
						order.Barrier();
					}
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
						barrier = true;
						break;

					case 'i':
						WriteInsert(value, spanOpen, spanClose - spanOpen);
						barrier = true;
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
							barrier = true;
						}
					}
					if (barrier)
					{
						order.Barrier();
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
						if (done)
						{
							order.Barrier();
						}
						return done;
					}
					var mode = value[spanOpen];
					spanOpen = value.IndexOf('>', spanOpen);
					if (spanOpen < 0 || spanOpen >= spanClose)
					{
						if (done)
						{
							order.Barrier();
						}
						return done;
					}
					++spanOpen;
					open = spanClose + 7;
					bool barrier;
					if (value[spanClose - 1] == ')')
					{
						--spanClose;
						barrier = true;
					}
					else
					{
						barrier = false;
					}
					//Process
					if (spanOpen != spanClose)
					{
						switch (mode)
						{
						case 'd':
							WriteDelete(value, spanOpen, spanClose - spanOpen);
							if (barrier)
							{
								order.Barrier();
							}
							break;

						case 'i':
							WriteInsert(value, spanOpen, spanClose - spanOpen);
							if (barrier)
							{
								order.Barrier();
							}
							break;

						default:
							break;
						}
					}
				}
				//Next
				if (open >= last)
				{
					if (done)
					{
						order.Barrier();
					}
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
