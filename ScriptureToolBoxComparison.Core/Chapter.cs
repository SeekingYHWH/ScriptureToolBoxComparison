using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class Chapter
	{
		private readonly string name;
		private readonly string source;

		public Chapter(string name, string source)
		{
			this.name = name;
			this.source = source;
		}

		public string Name => name;
		public string Source => source;
	}
}
