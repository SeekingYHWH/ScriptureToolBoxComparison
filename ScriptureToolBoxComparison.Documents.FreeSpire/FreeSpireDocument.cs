using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Formatting;

namespace ScriptureToolBoxComparison
{
	/// <summary>
	/// Limit of 500 paragraphs
	/// </summary>
	public sealed class FreeSpireDocument : IDocument
	{
		private readonly string path;
		private Document document;
		private CharacterFormat delete;
		private CharacterFormat insert;
		private CharacterFormat normal;
		private Section section;
		private Paragraph chapter;

		public FreeSpireDocument(XmlNode config, string path)
		{
			this.path = path;

			this.document = new Document();
			this.delete = new CharacterFormat(document) { Bold = false, IsStrikeout = true, DoubleStrike = true, };
			this.insert = new CharacterFormat(document) { Bold = true, IsStrikeout = false, DoubleStrike = false, };
			this.normal = new CharacterFormat(document) { Bold = false, IsStrikeout = false, DoubleStrike = false, };
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

			if (section != null)
			{
				section = document.AddSection();
				section.BreakCode = SectionBreakType.NoBreak;
			}
			document.SaveToFile(path, FileFormat.Docx2019);

			document = null;
		}

		public void BookStart(Book value)
		{
			if (section == null)
			{
				section = document.AddSection();
				section.BreakCode = SectionBreakType.NewPage;
				CreateHeaderFooter();
			}
			else
			{
				section = document.AddSection();
				section.BreakCode = SectionBreakType.NoBreak;
			}
			var paragraph = section.AddParagraph();
			paragraph.ApplyStyle(BuiltinStyle.Heading1);
			paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
			paragraph.AppendText(value.Name);
			section = document.AddSection();
			section.BreakCode = SectionBreakType.NoBreak;
			section.Columns.Add(new Column(document) { Width = 2.29f * 72, Space = 2.29f * 72, });
			section.Columns.Add(new Column(document) { Width = 2.29f * 72, Space = 2.29f * 72, });
		}

		public void BookFinish()
		{
		}

		public void ChapterStart(Chapter value)
		{
			var paragraph = section.AddParagraph();
			paragraph.ApplyStyle(BuiltinStyle.Heading3);
			paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
			paragraph.AppendText(value.Name);
			chapter = section.AddParagraph();
			chapter.Format.HorizontalAlignment = HorizontalAlignment.Justify;
		}

		public void ChapterFinish()
		{
		}

		public void WriteDelete(string value)
		{
			var range = chapter.AppendText(value);
			range.ApplyCharacterFormat(delete);
		}

		public void WriteInsert(string value)
		{
			var range = chapter.AppendText(value);
			range.ApplyCharacterFormat(insert);
		}

		public void WriteNormal(string value)
		{
			var range = chapter.AppendText(value);
			range.ApplyCharacterFormat(normal);
		}

		private void CreateHeaderFooter()
		{
			section.PageSetup.DifferentOddAndEvenPagesHeaderFooter = true;

			var of = section.HeadersFooters.OddFooter.AddParagraph();
			of.Format.HorizontalAlignment = HorizontalAlignment.Right;
			of.AppendField("page number", FieldType.FieldPage);

			var ef = section.HeadersFooters.EvenFooter.AddParagraph();
			ef.Format.HorizontalAlignment = HorizontalAlignment.Left;
			ef.AppendField("page number", FieldType.FieldPage);
		}
	}
}
