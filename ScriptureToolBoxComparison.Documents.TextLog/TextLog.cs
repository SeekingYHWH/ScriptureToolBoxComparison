using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	public sealed class TextLog : IDocument
	{
		private readonly StreamWriter writer;

		public TextLog(XmlNode config, string path)
		{
			var writerStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			this. writer = new StreamWriter(writerStream);
		}

		~TextLog()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			writer.Dispose();
		}

		public void BookStart(Book book)
		{
			writer.Write("   BOOK: ");
			writer.WriteLine(book.Name);
			writer.Flush();
		}

		public void BookFinish()
		{
			writer.WriteLine("   BOOK");
			writer.Flush();
		}

		public void ChapterStart(Chapter chapter)
		{
			writer.Write("CHAPTER: ");
			writer.WriteLine(chapter.Name);
			writer.Flush();
		}

		public void ChapterFinish()
		{
			writer.WriteLine("CHAPTER");
			writer.Flush();
		}

		public void WriteDelete(string text)
		{
			writer.Write(" DETELE: ");
			writer.WriteLine(text);
			writer.Flush();
		}

		public void WriteInsert(string text)
		{
			writer.Write(" INSERT: ");
			writer.WriteLine(text);
			writer.Flush();
		}

		public void WriteNormal(string text)
		{
			writer.Write(" NORMAL: ");
			writer.WriteLine(text);
			writer.Flush();
		}
	}
}
