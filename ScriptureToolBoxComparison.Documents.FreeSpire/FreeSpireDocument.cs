﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
		private SizeF pageSize;
		private float marginsGutter;
		private float marginsTop;
		private float marginsBottom;
		private float marginsLeft;
		private float marginsRight;
		private Column[] columns;
		private string fontName;
		private float fontSize;
		private CharacterFormat delete;
		private CharacterFormat insert;
		private CharacterFormat normal;
		private Section section;
		private Paragraph chapter;

		public FreeSpireDocument(XmlNode config, string path)
		{
			//TODO: Get settings from config
			this.path = path;

			this.document = new Document();
			document.EmbedFontsInFile = true;

			this.pageSize = PageSize.Letter;
			this.marginsGutter = 0.50f * 72;
			this.marginsTop = 0.25f * 72;
			this.marginsBottom = 0.25f * 72;
			this.marginsLeft = 0.30f * 72;
			this.marginsRight = 0.30f * 72;
			this.columns = new Column[]
			{
				new Column(document) { Width = 2.47f * 72, Space = 2.46f * 72, },
				new Column(document) { Width = 2.47f * 72, Space = 2.46f * 72, },
			};
			this.fontName = "Times New Roman";
			this.fontSize = 12;
			this.delete = new CharacterFormat(document) { Bold = false, IsStrikeout = true, DoubleStrike = false, FontName = fontName, FontSize = fontSize, };
			this.insert = new CharacterFormat(document) { Bold = true, IsStrikeout = false, DoubleStrike = false, FontName = fontName, FontSize = fontSize, };
			this.normal = new CharacterFormat(document) { Bold = false, IsStrikeout = false, DoubleStrike = false, FontName = fontName, FontSize = fontSize, };

			CreateStart();
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

			CreateEnding();
			document.UpdateTOCPageNumbers();
			document.SaveToFile(path, FileFormat.Docx2019);

			document = null;
		}

		public void BookStart(Book value)
		{
			if (section == null)
			{
				section = document.AddSection();
				section.BreakCode = SectionBreakType.NewPage;
				ConfigBody();
			}
			else
			{
				section = document.AddSection();
				section.BreakCode = SectionBreakType.NoBreak;
			}
			var paragraph = section.AddParagraph();
			paragraph.ApplyStyle(BuiltinStyle.Heading1);
			paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
			var range = paragraph.AppendText(value.Name);
			range.CharacterFormat.FontName = fontName;
			section = document.AddSection();
			section.BreakCode = SectionBreakType.NoBreak;
			foreach (var column in columns)
			{
				section.Columns.Add(column);
			}
		}

		public void BookFinish()
		{
		}

		public void ChapterStart(Chapter value)
		{
			var paragraph = section.AddParagraph();
			paragraph.ApplyStyle(BuiltinStyle.Heading4);
			paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
			var range = paragraph.AppendText(value.Name);
			range.CharacterFormat.FontName = fontName;
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

		private void CreateStart()
		{
			section = document.AddSection();
			section.BreakCode = SectionBreakType.NewPage;

			//Page
			ConfigPage(section.PageSetup);

			//Headers
			section.PageSetup.DifferentOddAndEvenPagesHeaderFooter = true;

			var oh = section.HeadersFooters.OddHeader.AddParagraph();
			oh.Format.HorizontalAlignment = HorizontalAlignment.Right;

			var of = section.HeadersFooters.OddFooter.AddParagraph();
			of.Format.HorizontalAlignment = HorizontalAlignment.Right;

			var eh = section.HeadersFooters.EvenHeader.AddParagraph();
			eh.Format.HorizontalAlignment = HorizontalAlignment.Left;

			var ef = section.HeadersFooters.EvenFooter.AddParagraph();
			ef.Format.HorizontalAlignment = HorizontalAlignment.Left;

			//Intro
			var paragraph = section.AddParagraph();
			paragraph.AppendBreak(BreakType.PageBreak);
			paragraph = section.AddParagraph();
			paragraph.AppendBreak(BreakType.PageBreak);
			paragraph = section.AddParagraph();
			paragraph.AppendBreak(BreakType.PageBreak);
			paragraph = section.AddParagraph();
			paragraph.AppendBreak(BreakType.PageBreak);

			//Table of Contents
			paragraph = section.AddParagraph();
			paragraph.Format.HorizontalAlignment = HorizontalAlignment.Center;
			var range = paragraph.AppendText("Contents");
			range.ApplyCharacterFormat(normal);
			paragraph = section.AddParagraph();
			var toc = paragraph.AppendTOC(1, 2);
			toc.CharacterFormat.FontName = fontName;
			paragraph.AppendBreak(BreakType.PageBreak);

			//Blank Page
			paragraph.AppendBreak(BreakType.PageBreak);

			//Prepare
			section = null;
		}

		private void ConfigBody()
		{
			//Page
			ConfigPage(section.PageSetup);

			//Numbers
			section.PageSetup.RestartPageNumbering = true;
			section.PageSetup.PageStartingNumber = 1;

			//Headers
			section.HeadersFooters.LinkToPrevious = false;
			section.PageSetup.DifferentOddAndEvenPagesHeaderFooter = true;

			var oh = section.HeadersFooters.OddHeader.FirstParagraph;
			oh.Format.HorizontalAlignment = HorizontalAlignment.Right;
			var oh1 = oh.AppendField("Heading 1", FieldType.FieldStyleRef);
			oh1.ApplyCharacterFormat(normal);
			var ohs = oh.AppendText(" ");
			ohs.ApplyCharacterFormat(normal);
			var oh4 = oh.AppendField("Heading 4", FieldType.FieldStyleRef);
			oh4.ApplyCharacterFormat(normal);

			var of = section.HeadersFooters.OddFooter.FirstParagraph;
			of.Format.HorizontalAlignment = HorizontalAlignment.Right;
			var ofp = of.AppendField("page number", FieldType.FieldPage);
			ofp.ApplyCharacterFormat(normal);

			var eh = section.HeadersFooters.EvenHeader.FirstParagraph;
			eh.Format.HorizontalAlignment = HorizontalAlignment.Left;
			var eh1 = eh.AppendField("Heading 1", FieldType.FieldStyleRef);
			eh1.ApplyCharacterFormat(normal);
			var ehs = eh.AppendText(" ");
			ehs.ApplyCharacterFormat(normal);
			var eh4 = eh.AppendField("Heading 4", FieldType.FieldStyleRef);
			eh4.ApplyCharacterFormat(normal);

			var ef = section.HeadersFooters.EvenFooter.FirstParagraph;
			ef.Format.HorizontalAlignment = HorizontalAlignment.Left;
			var efp = ef.AppendField("page number", FieldType.FieldPage);
			efp.ApplyCharacterFormat(normal);
		}

		private void CreateEnding()
		{
			if (section == null)
			{
				return;
			}

			section = document.AddSection();
			section.BreakCode = SectionBreakType.NoBreak;

			section = document.AddSection();
			section.HeadersFooters.LinkToPrevious = false;
			section.BreakCode = SectionBreakType.NewPage;

			ConfigPage(section.PageSetup);

			var paragraph = section.AddParagraph();
			paragraph.AppendBreak(BreakType.PageBreak);
		}

		private void ConfigPage(PageSetup pageSetup)
		{
			pageSetup.PageSize = pageSize;
			pageSetup.Gutter = marginsGutter;
			var margins = pageSetup.Margins;
			margins.Top = marginsTop;
			margins.Bottom = marginsBottom;
			margins.Left = marginsLeft;
			margins.Right = marginsRight;
		}
	}
}
