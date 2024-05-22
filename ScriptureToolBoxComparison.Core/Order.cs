using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptureToolBoxComparison
{
	public static class Order
	{
		public static bool NeedSpace(char letter)
		{
			switch (letter)
			{
			case '.':
			case ':':
			case ';':
			case ',':
				return false;

			default:
				return true;
			}
		}
	}
}
