using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class SortOrder : IOrder
	{
		private IDocument document;
		private readonly StringBuilder builder = new StringBuilder();
		private Wrote wrote;
		private readonly Queue<Segment> deletes = new Queue<Segment>();
		private readonly Queue<Segment> inserts = new Queue<Segment>();

		public SortOrder()
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
			deletes.Clear();
			inserts.Clear();
		}

		public void ChapterFinish()
		{
			throw new NotImplementedException();
		}

		public void WriteDelete(string value, int offset, int length)
		{
			WriteNormal();
			deletes.Enqueue(new Segment(value, offset, length));
		}

		public void WriteInsert(string value, int offset, int length)
		{
			WriteNormal();
			inserts.Enqueue(new Segment(value, offset, length));
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
			case Wrote.Insert:
				WriteOrdered();
				builder.Append(' ');
				wrote = Wrote.Normal;
				break;

			case Wrote.Normal:
				builder.Append(' ');
				break;
			}
			//Append
			builder.Append(value, offset, length);
		}

		private void WriteNormal()
		{
			if (wrote != Wrote.Normal)
			{
				return;
			}

			document.WriteNormal(builder.ToString());
			wrote = Wrote.Normal;
		}

		private void WriteOrdered()
		{
			WriteDelete();
		}

		private void WriteDelete()
		{
			if (!deletes.TryDequeue(out var value))
			{
				return;
			}
			if (wrote != Wrote.Delete)
			{
				builder.Append(' ');
			}
			while (true)
			{
				builder.Append(value.Value, value.Offset, value.Length);
				if (!deletes.TryDequeue(out value))
				{
					document.WriteDelete(builder.ToString());
					builder.Clear();
					wrote = Wrote.Delete;
					return;
				}
				builder.Append(' ');
			}
		}
	}
}
