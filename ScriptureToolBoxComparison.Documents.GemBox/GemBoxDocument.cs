using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using GemBox.Document;

using static System.Net.Mime.MediaTypeNames;

namespace ScriptureToolBoxComparison
{
	public class GemBoxDocument : IDocument
	{
		static GemBoxDocument()
		{
			//Free version only allows 20 Paragraph's
			ComponentInfo.SetLicense("FREE-LIMITED-KEY");
		}

		private readonly string path;
		private readonly DocumentModel document;
		private readonly CharacterFormat book;
		private readonly CharacterFormat chapter;
		private readonly CharacterFormat delete;
		private readonly CharacterFormat insert;
		private readonly CharacterFormat normal;
		private readonly Section section;
		private Paragraph paragraph;

		public GemBoxDocument(XmlNode config, string path)
		{
			this.path = path;

			this.document = new DocumentModel();
			var heading1 = Style.CreateStyle(StyleTemplateType.Heading1, document);
			//heading1.CharacterFormat = new CharacterFormat() { };
			document.Styles.Add(heading1);
			var heading3 = Style.CreateStyle(StyleTemplateType.Heading3, document);
			//heading3.CharacterFormat = new CharacterFormat() { };
			document.Styles.Add(heading3);
			this.book = new CharacterFormat();// { Style = heading1, };
			this.chapter = new CharacterFormat();// { Style = heading3, };
			this.delete = new CharacterFormat() { Bold = false, Strikethrough = true, };
			this.insert = new CharacterFormat() { Bold = true, Strikethrough = false, };
			this.normal = new CharacterFormat() { Bold = false, Strikethrough = false, };
			this.section = new Section(document);
			section.PageSetup.PaperType = PaperType.Letter;
			document.Sections.Add(section);
		}

		~GemBoxDocument()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			document.Save(path);
		}

		public void BookStart(Book value)
		{
			paragraph = new Paragraph(document);
			section.Blocks.Add(paragraph);
			var run = new Run(document);
			run.CharacterFormat = delete.Clone();
			run.Text = value.Name;
			paragraph.Inlines.Add(run);
		}

		public void BookFinish()
		{
		}

		public void ChapterStart(Chapter value)
		{
			paragraph = new Paragraph(document);
			section.Blocks.Add(paragraph);
			var run = new Run(document);
			run.CharacterFormat = chapter.Clone();
			run.Text = value.Name;
			paragraph.Inlines.Add(run);
			paragraph = new Paragraph(document);
			section.Blocks.Add(paragraph);
		}

		public void ChapterFinish()
		{
		}

		public void WriteDelete(string value)
		{
			var run = new Run(document);
			run.CharacterFormat = delete.Clone();
			run.Text = value;
			paragraph.Inlines.Add(run);
		}

		public void WriteInsert(string value)
		{
			var run = new Run(document);
			run.CharacterFormat = insert.Clone();
			run.Text = value;
			paragraph.Inlines.Add(run);
		}

		public void WriteNormal(string value)
		{
			var run = new Run(document);
			run.CharacterFormat = normal.Clone();
			run.Text = value;
			paragraph.Inlines.Add(run);
		}
	}
}
