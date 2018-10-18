using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TreeOfKnowledge
{
    class TOKNode
    {
        public String Name;
        public List<String> Tags { get; } = new List<String>();

        public TOKNode(String name)
        {
            Name = name;
        }
    }
}