using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	internal sealed class Segment
	{
		private readonly string value;
		private readonly int offset;
		private readonly int length;

		public Segment(string value, int offset, int length)
		{
			this.value = value;
			this.offset = offset;
			this.length = length;
		}

		public string Value => value;
		public int Offset => offset;
		public int Length => length;
	}
}
