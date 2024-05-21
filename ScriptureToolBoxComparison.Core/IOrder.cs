using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public interface IOrder
	{
		void SetDocument(IDocument document);
		void ChapterStart(Chapter chapter);
		void ChapterFinish();
		void WriteDelete(string value, int offset, int length);
		void WriteInsert(string value, int offset, int length);
		void WriteNormal(string value, int offset, int length);
		void Barrier();
	}
}
