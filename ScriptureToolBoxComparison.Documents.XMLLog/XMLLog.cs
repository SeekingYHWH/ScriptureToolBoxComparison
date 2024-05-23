using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	public sealed class XMLLog : IDocument
	{
		private readonly XmlTextWriter writer;

		public XMLLog(XmlNode config, string path)
		{
			var writerStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			var writerText = new StreamWriter(writerStream);
			this.writer = new XmlTextWriter(writerText);
			this.writer.Formatting = Formatting.Indented;
			this.writer.IndentChar = '\t';

			writer.WriteStartElement("Log");
		}

		~XMLLog()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			writer.WriteEndElement();
			writer.Dispose();
		}

		public void BookStart(Book book)
		{
			writer.WriteStartElement("Book");
			writer.WriteAttributeString("Name", book.Name);
		}

		public void BookFinish()
		{
			writer.WriteEndElement();
		}

		public void ChapterStart(Chapter chapter)
		{
			writer.WriteStartElement("Chapter");
			writer.WriteAttributeString("Name", chapter.Name);
		}

		public void ChapterFinish()
		{
			writer.WriteEndElement();
		}

		public void WriteDelete(string text)
		{
			writer.WriteElementString("Delete", text);
		}

		public void WriteInsert(string text)
		{
			writer.WriteElementString("Insert", text);
		}

		public void WriteNormal(string text)
		{
			writer.WriteElementString("Normal", text);
		}
	}
}
