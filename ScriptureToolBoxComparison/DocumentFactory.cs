using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	internal static class DocumentFactory
	{
		public static IDocument Create(string configPath, string path)
		{
			var doc = new XmlDocument();
			doc.Load(configPath);
			var config = doc.DocumentElement;
			var typeConfig = config.Attributes["Type"];
			if (typeConfig == null)
			{
				throw new ArgumentException("Missing Type");
			}
			var type = typeConfig.InnerText;
			switch (type)
			{
			case "null":
				return CreateNull(config, path);

			default:
				throw new ArgumentException("Invalid Type");
			}
		}

		public static IDocument CreateNull(XmlNode config, string path)
		{
			return new NullDocument();
		}
	}
}
