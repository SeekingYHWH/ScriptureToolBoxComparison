using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public sealed class Book
	{
		private readonly string name;
		private readonly Chapter[] chapters;

		public Book(string name)
		{
			this.name = name;
		}

		public Book(string name, Chapter[] chapters)
		{
			this.name = name;
			this.chapters = chapters;
		}

		public string Name => name;
		public Chapter[] Chapters => chapters;
	}
}
