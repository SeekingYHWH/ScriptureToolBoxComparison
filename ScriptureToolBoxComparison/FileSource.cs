using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScriptureToolBoxComparison
{
	internal sealed class FileSource : ISource
	{
		private readonly string path;

		public FileSource(string path)
		{
			this.path = path;
		}

		public Stream GetStream(Chapter value)
		{
			var streamPath = Path.Combine(path, value.Source);
			var stream = new FileStream(streamPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return stream;
		}
	}
}
