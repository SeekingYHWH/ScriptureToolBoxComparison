using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class MultipleDocument : IDocument
	{
		private readonly IDocument[] documents;

		public MultipleDocument(params IDocument[] documents)
		{
			this.documents = documents;
		}

		~MultipleDocument()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			foreach (var document in documents)
			{
				document.Dispose();
			}
		}

		public void BookStart(Book book)
		{
			foreach (var document in documents)
			{
				document.BookStart(book);
			}
		}

		public void BookFinish()
		{
			foreach (var document in documents)
			{
				document.BookFinish();
			}
		}

		public void ChapterStart(Chapter chapter)
		{
			foreach (var document in documents)
			{
				document.ChapterStart(chapter);
			}
		}

		public void ChapterFinish()
		{
			foreach (var document in documents)
			{
				document.ChapterFinish();
			}
		}

		public void WriteDelete(string text)
		{
			foreach (var document in documents)
			{
				document.WriteDelete(text);
			}
		}

		public void WriteInsert(string text)
		{
			foreach (var document in documents)
			{
				document.WriteInsert(text);
			}
		}

		public void WriteNormal(string text)
		{
			foreach (var document in documents)
			{
				document.WriteNormal(text);
			}
		}
	}
}
