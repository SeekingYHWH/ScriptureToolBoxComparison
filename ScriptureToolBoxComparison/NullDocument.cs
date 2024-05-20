using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureToolBoxComparison
{
	internal sealed class NullDocument : IDocument
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

		public void BookFinish()
		{
		}

		public void BookStart(Book book)
		{
		}

		public void ChapterFinish()
		{
		}

		public void ChapterStart(Chapter chapter)
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
