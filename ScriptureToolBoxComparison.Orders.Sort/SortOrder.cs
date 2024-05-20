using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			//Delete
			WriteDelete();
			//Insert
			WriteInsert();
			//Normal
			document.WriteNormal(builder.ToString());
			builder.Clear();
			wrote = Wrote.Normal;
		}

		public void WriteDelete(string value, int offset, int length)
		{
			//Normal
			WriteNormal();
			//Delete
			deletes.Enqueue(new Segment(value, offset, length));
		}

		public void WriteInsert(string value, int offset, int length)
		{
			//Normal
			WriteNormal();
			//Insert
			inserts.Enqueue(new Segment(value, offset, length));
		}

		public void WriteNormal(string value, int offset, int length)
		{
			//Sort
			WriteDelete();
			WriteInsert();
            //State
            switch (wrote)
			{
			default:
				wrote = Wrote.Normal;
				break;

			case Wrote.Delete:
			case Wrote.Insert:
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteNormal()
		{
			if (builder.Length > 0)
			{
				builder.Append(' ');
				document.WriteNormal(builder.ToString());
				builder.Clear();
			}
		}

		private void WriteDelete()
		{
			if (!deletes.TryDequeue(out var value))
			{
				return;
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

		private void WriteInsert()
		{
			if (!inserts.TryDequeue(out var value))
			{
				return;
			}
			if (wrote == Wrote.Delete)
			{
				document.WriteNormal(" ");
			}
			while (true)
			{
				builder.Append(value.Value, value.Offset, value.Length);
				if (!deletes.TryDequeue(out value))
				{
					document.WriteInsert(builder.ToString());
					builder.Clear();
					wrote = Wrote.Insert;
					return;
				}
				builder.Append(' ');
			}
		}
	}
}
