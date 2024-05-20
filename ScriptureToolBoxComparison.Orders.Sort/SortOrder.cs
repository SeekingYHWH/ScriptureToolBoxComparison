using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class SortOrder : IOrder
	{
		private IDocument document;

		public SortOrder()
		{
		}

		public void SetDocument(IDocument document)
		{
			this.document = document;
		}

		public void ChapterStart(Chapter chapter)
		{
		}

		public void ChapterFinish()
		{
		}

		public void BookStart(Book book)
		{
		}

		public void BookFinish()
		{
		}

		public void WriteDelete(string value, int offset, int length)
		{
		}

		public void WriteInsert(string value, int offset, int length)
		{
		}

		public void WriteNormal(string value, int offset, int length)
		{
		}
	}
}
