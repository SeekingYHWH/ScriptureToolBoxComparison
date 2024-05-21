﻿using System;
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
		private Wrote barrier;

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
			barrier = Wrote.None;
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
			switch (barrier)
			{
			case Wrote.Delete:
				deletes.Enqueue(new Segment(value, offset, length));
				return;

			case Wrote.Insert:
				throw new NotImplementedException();

			default:
				WriteNormal();
				deletes.Enqueue(new Segment(value, offset, length));
				return;
			}
		}

		public void WriteInsert(string value, int offset, int length)
		{
			switch (barrier)
			{
			case Wrote.Delete:
				barrier = Wrote.Insert;
				inserts.Enqueue(new Segment(value, offset, length));
				return;

			case Wrote.Insert:
				inserts.Enqueue(new Segment(value, offset, length));
				return;

			default:
				WriteNormal();
				inserts.Enqueue(new Segment(value, offset, length));
				return;
			}
		}

		public void WriteNormal(string value, int offset, int length)
		{
			//Sort
			WriteDelete();
			WriteInsert();
			//State
			barrier = Wrote.None;
            switch (wrote)
			{
			default:
				wrote = Wrote.Normal;
				builder.Append(value, offset, length);
				return;

			case Wrote.Delete:
			case Wrote.Insert:
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
			if (inserts.Count > 0)
			{
				barrier = Wrote.Insert;
				return;
			}
			if (deletes.Count > 0)
			{
				barrier = Wrote.Delete;
				return;
			}
			barrier = Wrote.None;
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
				if (!inserts.TryDequeue(out value))
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
