﻿using System;
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
		private Wrote barrier;
		private int barrierCount;

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
			WriteNormalSort();
		}

		public void WriteDelete(string value, int offset, int length)
		{
			switch (barrier)
			{
			case Wrote.Delete:
				deletes.Enqueue(new Segment(value, offset, length));
				return;

			case Wrote.Insert:
				WriteDeleteInsert();
				if (Order.NeedSpace(value[offset]))
				{
					document.WriteNormal(" ");
				}
				barrier = Wrote.None;
				deletes.Enqueue(new Segment(value, offset, length));
				return;

			default:
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
				barrierCount = 0;
				inserts.Enqueue(new Segment(value, offset, length));
				return;

			case Wrote.Insert:
				inserts.Enqueue(new Segment(value, offset, length));
				return;

			default:
				inserts.Enqueue(new Segment(value, offset, length));
				return;
			}
		}

		public void WriteNormal(string value, int offset, int length)
		{
			//Sort
			WriteNormalSort();
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
				if (Order.NeedSpace(value[offset]))
				{
					builder.Append(' ');
				}
				builder.Append(value, offset, length);
				return;

			case Wrote.Normal:
				if (Order.NeedSpace(value[offset]))
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
				barrierCount = inserts.Count;
				return;
			}
			if (deletes.Count > 0)
			{
				barrier = Wrote.Delete;
				return;
			}
			barrier = Wrote.None;
		}

		private void WriteNormalSort()
		{
			if (builder.Length > 0)
			{
				if (deletes.TryPeek(out var value))
				{
					if (Order.NeedSpace(value.Value[value.Offset]))
					{
						builder.Append(' ');
					}
				}
				else if (inserts.TryPeek(out value))
				{
					if (Order.NeedSpace(value.Value[value.Offset]))
					{
						builder.Append(' ');
					}
				}
				document.WriteNormal(builder.ToString());
				builder.Clear();
			}

			WriteDelete();
			WriteInsert();
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
				if (Order.NeedSpace(value.Value[value.Offset]))
				{
					builder.Append(' ');
				}
			}
		}

		private void WriteDeleteInsert()
		{
			Segment value;

			//Normal
			if (builder.Length > 0)
			{
				if (deletes.TryPeek(out value))
				{
					if (Order.NeedSpace(value.Value[value.Offset]))
					{
						builder.Append(' ');
					}
				}
				else if (inserts.TryPeek(out value))
				{
					if (Order.NeedSpace(value.Value[value.Offset]))
					{
						builder.Append(' ');
					}
				}
				document.WriteNormal(builder.ToString());
				builder.Clear();
			}

			//Delete
			WriteDelete();

			//Insert
			value = inserts.Dequeue();
			--barrierCount;
			if (wrote == Wrote.Delete)
			{
				if (Order.NeedSpace(value.Value[value.Offset]))
				{
					document.WriteNormal(" ");
				}
			}
			while (true)
			{
				builder.Append(value.Value, value.Offset, value.Length);
				if (barrierCount <= 0 || !inserts.TryDequeue(out value))
				{
					document.WriteInsert(builder.ToString());
					builder.Clear();
					wrote = Wrote.Insert;
					return;
				}
				--barrierCount;
				if (Order.NeedSpace(value.Value[value.Offset]))
				{
					builder.Append(' ');
				}
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
				if (Order.NeedSpace(value.Value[value.Offset]))
				{
					document.WriteNormal(" ");
				}
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
				if (Order.NeedSpace(value.Value[value.Offset]))
				{
					builder.Append(' ');
				}
			}
		}
	}
}
