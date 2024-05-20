using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class OriginalOrder : IOrder
	{
		private IDocument document;
		private readonly StringBuilder builder = new StringBuilder();
		private Wrote wrote;

		public OriginalOrder()
		{
		}

		public void SetDocument(IDocument document)
		{
			this.document = document;
		}

		public void ChapterStart(Chapter chapter)
		{
			builder.Clear();
			wrote = Wrote.None;
		}

		public void ChapterFinish()
		{
			//State
			switch (wrote)
			{
			default:
				break;

			case Wrote.Delete:
				document.WriteDelete(builder.ToString());
				builder.Clear();
				break;

			case Wrote.Insert:
				document.WriteInsert(builder.ToString());
				builder.Clear();
				break;

			case Wrote.Normal:
				document.WriteNormal(builder.ToString());
				builder.Clear();
				break;
			}
		}

		public void WriteDelete(string value, int offset, int length)
		{
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
				document.WriteInsert(builder.ToString());
				builder.Clear();
				document.WriteNormal(" ");
				wrote = Wrote.Delete;
				break;

			case Wrote.Normal:
				builder.Append(' ');
				document.WriteNormal(builder.ToString());
				builder.Clear();
				wrote = Wrote.Delete;
				break;
			}
			//Append
			builder.Append(value, offset, length);
		}

		public void WriteInsert(string value, int offset, int length)
		{
			switch (wrote)
			{
			default:
				wrote = Wrote.Insert;
				break;

			case Wrote.Delete:
				document.WriteDelete(builder.ToString());
				builder.Clear();
				document.WriteNormal(" ");
				wrote = Wrote.Insert;
				break;

			case Wrote.Insert:
				builder.Append(' ');
				break;

			case Wrote.Normal:
				builder.Append(' ');
				document.WriteNormal(builder.ToString());
				builder.Clear();
				wrote = Wrote.Insert;
				break;
			}
			//Append
			builder.Append(value, offset, length);
		}

		public void WriteNormal(string value, int offset, int length)
		{
			//State
			switch (wrote)
			{
			default:
				wrote = Wrote.Normal;
				break;

			case Wrote.Delete:
				document.WriteDelete(builder.ToString());
				builder.Clear();
				wrote = Wrote.Normal;
				builder.Append(' ');
				break;

			case Wrote.Insert:
				document.WriteInsert(builder.ToString());
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
	}
}
