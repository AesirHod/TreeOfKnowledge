using System;
using System.Collections.Generic;
namespace TreeOfKnowledge
{
    class TOKBranch : TOKNode
    {
        public List<TOKNode> Nodes { get; } = new List<TOKNode>();

        protected TOKBranch(String name)
        : base(name)
        {
        }

        public TOKBranch()
        : base("Branch")
        {
        }
    }
}