using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public interface IDocument : IDisposable
	{
		void BookStart(Book book);
		void BookFinish();
		void ChapterStart(Chapter chapter);
		void ChapterFinish();
		void WriteDelete(string text);
		void WriteInsert(string text);
		void WriteNormal(string text);
	}
}
