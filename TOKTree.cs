using System;
using System.Xml;

namespace TreeOfKnowledge
{
    class TOKTree
    {
        String _fileName;
        public TOKRoot Root { get; } = new TOKRoot();

        public TOKTree(String fileName)
        {
            _fileName = fileName;
        }

        public void Open(String fileName)
        {
            _fileName = fileName;
            Root.Nodes.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load(_fileName);

            XmlNode xmlRoot = doc.DocumentElement;
            AppendTOKChildren(doc, xmlRoot, Root);
        }

        public void SaveAs(String fileName)
        {
            _fileName = fileName;

            XmlDocument doc = new XmlDocument();
            XmlNode declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            XmlNode xmlRoot = doc.CreateElement("TOKRoot");
            AppendXMLChildren(doc, xmlRoot, Root);
            doc.AppendChild(xmlRoot);
            
            doc.Save(fileName);
        }

        private void AppendTOKChildren(XmlDocument doc, XmlNode xmlParent, TOKBranch tokParent)
        {
            foreach (XmlNode xmlNode in xmlParent.ChildNodes)
            {
                if (xmlNode.NodeType == XmlNodeType.Element)
                {
                    TOKNode tokNode = null;
                    if (xmlNode.Name == "TOKBranch")
                    {
                        TOKBranch tokBranch = new TOKBranch();
                        AppendTOKChildren(doc, xmlNode, tokBranch);
                        tokNode = tokBranch;
                    }
                    else
                    {
                        TOKLeaf tokLeaf = new TOKLeaf();
                        if (xmlNode.FirstChild?.NodeType == XmlNodeType.Text)
                        {
                            XmlText xmlText = (XmlText)xmlNode.FirstChild;
                            tokLeaf.Text = xmlText.Value;
                            tokNode = tokLeaf;
                        }
                    }

                    XmlNode xmlName = xmlNode.Attributes.GetNamedItem("name");
                    if (xmlName != null)
                    {
                        tokNode.Name = xmlName.Value;
                    }

                    tokParent.Nodes.Add(tokNode);
                }
            }
        }

        private void AppendXMLChildren(XmlDocument doc, XmlNode xmlParent, TOKBranch tokParent)
        {
            foreach (TOKNode tokNode in tokParent.Nodes)
            {
                XmlNode xmlNode = null;
                if (tokNode.GetType() == typeof(TOKBranch))
                {
                    xmlNode = doc.CreateElement("TOKBranch");
                    AppendXMLChildren(doc, xmlNode, (TOKBranch)tokNode);
                }
                else
                {
                    xmlNode = doc.CreateElement("TOKLeaf");
                    String nodeText = ((TOKLeaf)tokNode).Text;
                    if (nodeText == null)
                    {
                        nodeText = "";
                    }
                    XmlNode xmlText = doc.CreateTextNode(nodeText);
                    xmlNode.AppendChild(xmlText);
                }

                XmlNode xmlName = doc.CreateAttribute("name");
                xmlName.Value = tokNode.Name;
                xmlNode.Attributes.SetNamedItem(xmlName);

                xmlParent.AppendChild(xmlNode);
            }
        }
    }
}