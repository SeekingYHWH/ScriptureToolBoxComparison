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
			switch (wrote)
			{
			default:
				wrote = Wrote.Delete;
				builder.Append(value, offset, length);
				return;

			case Wrote.Delete:
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				builder.Append(value, offset, length);
				return;

			case Wrote.Insert:
				document.WriteInsert(builder.ToString());
				builder.Clear();
				if (Document.NeedSpace(value[offset]))
				{
					document.WriteNormal(" ");
				}
				wrote = Wrote.Delete;
				builder.Append(value, offset, length);
				return;

			case Wrote.Normal:
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				document.WriteNormal(builder.ToString());
				builder.Clear();
				wrote = Wrote.Delete;
				builder.Append(value, offset, length);
				return;
			}
		}

		public void WriteInsert(string value, int offset, int length)
		{
			switch (wrote)
			{
			default:
				wrote = Wrote.Insert;
				builder.Append(value, offset, length);
				return;

			case Wrote.Delete:
				document.WriteDelete(builder.ToString());
				builder.Clear();
				if (Document.NeedSpace(value[offset]))
				{
					document.WriteNormal(" ");
				}
				wrote = Wrote.Insert;
				builder.Append(value, offset, length);
				return;

			case Wrote.Insert:
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				builder.Append(value, offset, length);
				return;

			case Wrote.Normal:
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				document.WriteNormal(builder.ToString());
				builder.Clear();
				wrote = Wrote.Insert;
				builder.Append(value, offset, length);
				return;
			}
		}

		public void WriteNormal(string value, int offset, int length)
		{
			switch (wrote)
			{
			default:
				wrote = Wrote.Normal;
				builder.Append(value, offset, length);
				return;

			case Wrote.Delete:
				document.WriteDelete(builder.ToString());
				builder.Clear();
				wrote = Wrote.Normal;
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				builder.Append(value, offset, length);
				return;

			case Wrote.Insert:
				document.WriteInsert(builder.ToString());
				builder.Clear();
				wrote = Wrote.Normal;
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				builder.Append(value, offset, length);
				return;

			case Wrote.Normal:
				if (Document.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				builder.Append(value, offset, length);
				return;
			}
		}

		public void Barrier()
		{
		}
	}
}
