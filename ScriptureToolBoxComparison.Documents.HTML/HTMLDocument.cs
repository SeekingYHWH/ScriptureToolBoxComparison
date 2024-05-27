using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	public sealed class HTMLDocument : IDocument
	{
		private readonly string bookOpen = "<h1 style=\"text-align: center;\">";
		private readonly string bookClose = "</h1>";
		private readonly string chapterOpen = "<h4 style=\"text-align: center;\">Chapter ";
		private readonly string chapterClose = "</h4>";
		private readonly string versesOpen = "<p style=\"text-align: justify;\">";
		private readonly string versesClose = "</p>";
		private readonly string deleteOpen = "<span style=\"text-decoration: line-through;\">";
		private readonly string deleteClose = "</span>";
		private readonly string insertOpen = "<span style=\"font-weight: bold;\">";
		private readonly string insertClose = "</span>";
		private readonly string normalOpen = "<span>";
		private readonly string normalClose = "</span>";

		private readonly StreamWriter writer;
		private readonly Stack<string> closes = new Stack<string>();

		public HTMLDocument(XmlNode config, string path)
		{
			var writerStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			this.writer = new StreamWriter(writerStream);
			writer.WriteLine("<html>");
			closes.Push("</html>");
			writer.WriteLine("<body>");
			closes.Push("</body>");
		}

		~HTMLDocument()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			while (closes.TryPop(out var close))
			{
				writer.WriteLine(close);
			}
			writer.Dispose();
		}

		public void BookStart(Book book)
		{
			writer.Write(bookOpen);
			writer.Write(book.Name);
			writer.WriteLine(bookClose);
		}

		public void BookFinish()
		{
		}

		public void ChapterStart(Chapter chapter)
		{
			writer.Write(chapterOpen);
			writer.Write(chapter.Name);
			writer.WriteLine(chapterClose);
			writer.Write(versesOpen);
			closes.Push(versesClose);
		}

		public void ChapterFinish()
		{
			if (closes.TryPop(out var close))
			{
				writer.WriteLine(close);
			}
		}

		public void WriteDelete(string text)
		{
			writer.Write(deleteOpen);
			writer.Write(text);
			writer.Write(deleteClose);
		}

		public void WriteInsert(string text)
		{
			writer.Write(insertOpen);
			writer.Write(text);
			writer.Write(insertClose);
		}

		public void WriteNormal(string text)
		{
			writer.Write(normalOpen);
			writer.Write(text);
			writer.Write(normalClose);
		}
	}
}
