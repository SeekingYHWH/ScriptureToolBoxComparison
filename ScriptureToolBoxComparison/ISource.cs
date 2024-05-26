using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScriptureToolBoxComparison
{
	internal interface ISource
	{
		Stream GetStream(Chapter value);
	}
}
