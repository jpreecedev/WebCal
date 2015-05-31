namespace Translation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Properties;

    internal class Program
    {
        private static void Main()
        {
            var path = "C:\\Source\\TachographReader\\TachographReader.Shared\\Properties\\Resources.resx";
            var path2 = "C:\\Source\\TachographReader\\TachographReader.Shared\\Properties\\Resources.ru.resx";

            Console.WriteLine("Do you want to generate a translation file? (Y/N)");

            bool generateTranslationFile = Console.ReadLine().ToUpper() == "Y";
            if (generateTranslationFile)
            {
                Console.WriteLine("Enter the path of the resources file to translate:");
                //var resourcesPath = Console.ReadLine();
                //if (!string.IsNullOrEmpty(resourcesPath) && File.Exists(resourcesPath))
                //{
                GenerateTranslationFile(path, path2);
                Console.WriteLine("The translation file has been saved to your desktop.");
                //}
            }
            else
            {
                Console.WriteLine("Do you want to read an existing translation file? (Y/N)");
                var readTranslationFile = Console.ReadLine().ToUpper() == "Y";
                if (readTranslationFile)
                {
                    Console.WriteLine("Enter the path of the translation file to read:");
                    var translationPath = Console.ReadLine();
                    ReadTranslationFile(translationPath);
                    Console.WriteLine("The resources file has been saved to your desktop.");
                }
            }

            Console.ReadLine();
        }

        private static void GenerateTranslationFile(string path, string path2)
        {
            var document = new XmlDocument();
            document.Load(path);

            var russianDocument = new XmlDocument();
            russianDocument.Load(path2);

            var dataElements = document.GetElementsByTagName("data");
            var russianDataElements = russianDocument.GetElementsByTagName("data");

            var sb = new StringBuilder();
            sb.AppendLine("Name,English,Translation");

            foreach (XmlElement dataElement in dataElements)
            {
                var name = dataElement.GetAttribute("name");
                var value = dataElement.GetElementsByTagName("value")[0].InnerText;

                var translation = FindDataElementValue(russianDataElements, name);

                sb.AppendLine(string.Format("{0},\"{1}\",\"{2}\"", name, value, translation));
            }

            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Format("{0}.csv", Path.GetFileNameWithoutExtension(path))), sb.ToString(), Encoding.UTF8);
        }

        private static string FindDataElementValue(XmlNodeList dataElements, string name)
        {
            foreach (XmlElement dataElement in dataElements)
            {
                var elementName = dataElement.GetAttribute("name");
                if (elementName == name)
                {
                    return dataElement.GetElementsByTagName("value")[0].InnerText;
                }
            }

            return string.Empty;
        }

        private static void ReadTranslationFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Resources.TXT_DOCUMENT_HEADER);
            sb.Append(Resources.TXT_DOCUMENT_SCHEMA);

            var fileContents = File.ReadAllLines(path);
            for (int i = 1; i < fileContents.Length; i++)
            {
                var line = fileContents[i];
                var split = line.Split(',');

                var name = split[0];
                var translation = string.Empty;

                if (split.Length >= 3 && !string.IsNullOrEmpty(split[2]))
                {
                    translation = split[2];
                    sb.AppendFormat("<data name=\"{0}\" xml:space=\"preserve\"><value>{1}</value></data>", name, translation);
                }
            }

            sb.Append(Resources.TXT_DOCUMENT_FOOTER);
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Format("{0}.resx", Path.GetFileNameWithoutExtension(path))), sb.ToString());
        }
    }
}