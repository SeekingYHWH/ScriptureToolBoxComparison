using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ScriptureToolBoxComparison
{
	public static class BooksFactory
	{
		private static void LoadBooks(List<Book> books, string configPath)
		{
			var doc = new XmlDocument();
			doc.Load(configPath);
			var config = doc.DocumentElement;
			var chapters = new List<Chapter>();
			foreach (XmlNode bookConfig in config.SelectNodes("Book"))
			{
				var bookName = bookConfig.Attributes["Name"].InnerText;
				foreach (XmlNode chapterConfig in bookConfig.SelectNodes("Chapter"))
				{
					var chapterName = chapterConfig.Attributes["Name"].InnerText;
					var chapterSource = chapterConfig.Attributes["Source"].InnerText;
					var chapter = new Chapter(chapterName, chapterSource);
					chapters.Add(chapter);
				}
				var bookChapters = chapters.ToArray();
				chapters.Clear();
				var book = new Book(bookName, bookChapters);
				books.Add(book);
			}
		}
	}
}
