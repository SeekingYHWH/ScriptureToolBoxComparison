using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	public static class DocumentFactory
	{
		public static IDocument Create(string configPath, string path)
		{
			var doc = new XmlDocument();
			doc.Load(configPath);
			var config = doc.DocumentElement;
			throw new NotImplementedException();
		}
	}
}
