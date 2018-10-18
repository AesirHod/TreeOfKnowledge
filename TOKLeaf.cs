using System;

namespace TreeOfKnowledge
{
    class TOKLeaf : TOKNode
    {
        public String Text = "";

        public TOKLeaf()
        : base("Leaf")
        {
        }
    }
}