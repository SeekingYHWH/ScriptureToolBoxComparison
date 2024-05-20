using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class NullDocument : IDocument
	{
		public NullDocument()
		{
		}

		~NullDocument()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
		}

		public void BookStart(Book book)
		{
		}

		public void BookFinish()
		{
		}

		public void ChapterStart(Chapter chapter)
		{
		}

		public void ChapterFinish()
		{
		}

		public void WriteDelete(string text)
		{
		}

		public void WriteInsert(string text)
		{
		}

		public void WriteNormal(string text)
		{
		}
	}
}
