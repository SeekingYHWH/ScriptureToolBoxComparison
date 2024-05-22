using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Spire.Doc;
using Spire.Doc.Documents;

namespace ScriptureToolBoxComparison
{
	/// <summary>
	/// Limit of 500 paragraphs
	/// </summary>
	public sealed class FreeSpireDocument : IDocument
	{
		private readonly string path;
		private Document? document;
		private Section? section;
		private Paragraph chapter;

		public FreeSpireDocument(XmlNode config, string path)
		{
			this.path = path;

			this.document = new Document();
			this.section = document.AddSection();
		}

		~FreeSpireDocument()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
            if (document == null)
            {
				return;
            }

            document.SaveToFile(path, FileFormat.Docx2019);

			document = null;
			section = null;
		}

		public void BookStart(Book value)
		{
			var paragraph = section!.AddParagraph();
			paragraph.ApplyStyle(BuiltinStyle.Heading1);
			paragraph.Format.TextAlignment = TextAlignment.Center;
			paragraph.AppendText(value.Name);
		}

		public void BookFinish()
		{
		}

		public void ChapterStart(Chapter value)
		{
			var paragraph = section!.AddParagraph();
			paragraph.ApplyStyle(BuiltinStyle.Heading3);
			paragraph.Format.TextAlignment = TextAlignment.Center;
			paragraph.AppendText(value.Name);
			chapter = section.AddParagraph();
		}

		public void ChapterFinish()
		{
		}

		public void WriteDelete(string value)
		{
			chapter.AppendText(value);
		}

		public void WriteInsert(string value)
		{
			chapter.AppendText(value);
		}

		public void WriteNormal(string value)
		{
			chapter.AppendText(value);
		}
	}
}
