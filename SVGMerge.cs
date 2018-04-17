using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SVGMerge {
    class SVNMerge {
        static int Main(string[] args) {
            if (args.Length > 0) {
                Console.WriteLine(MergeData.HELP);
                return 0;
            }
            var prefix = (args.Length > 0 ? args[0] + "-{0}" : "{0}");
            const string CLASS_NAME="i";
            StringBuilder content = new StringBuilder(MergeData.XML_TEMPLATE_BEFORE);
            foreach (string filePath in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.svg")) {
                var document = new XmlDocument();
                document.Load(filePath);
                foreach (XmlElement mainElement in document.GetElementsByTagName("svg")) {
                    string fullPrefix = string.Format(prefix, Path.GetFileNameWithoutExtension(filePath).Replace(' ', '-'));
                    mainElement.SetAttribute("id", fullPrefix);
                    mainElement.SetAttribute("class", CLASS_NAME);
                    foreach (XmlNode styleElement in document.GetElementsByTagName("style")) {
                        const string REGEX = @"(?<class>.+?)(?<css>\{.*?})";
                        StringBuilder styles = new StringBuilder();
                        foreach (Match match in Regex.Matches(styleElement.InnerText, REGEX)) {
                            styles.Append("#").Append(fullPrefix).Append(" ")
                                .Append(match.Groups["class"]
                                .Value.Replace(",", ",#" + fullPrefix + " "))
                                .Append(match.Groups["css"]);
                        }
                        styleElement.InnerText = styles.ToString();
                    }
                }
                content.Append(document.InnerXml);
            }
            content.Append(MergeData.XML_TEMPLATE_AFTER);
            Console.Write(content.ToString().Replace("\n", ""));
            return 0;
        }
        
    }
}
