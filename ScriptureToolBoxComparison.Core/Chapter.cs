using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class Chapter
	{
		private readonly int name;
		private readonly string source;

		public Chapter(int name, string source)
		{
			this.name = name;
			this.source = source;
		}

		public int Name => name;
		public string Source => source;
	}
}
