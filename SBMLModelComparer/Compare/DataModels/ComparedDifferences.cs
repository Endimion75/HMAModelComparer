using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class ModifierDifferences
    {
        public List<string> ModifiersInANotInB { get; set; }

        public List<string> ModifiersInBNotInA { get; set; }

        public ModifierDifferences()
        {
            ModifiersInANotInB = new List<string>();
            ModifiersInBNotInA = new List<string>();
        }

        public ModifierDifferences(List<string> aNotInB, List<string> bNotinA)
        {
            ModifiersInANotInB = aNotInB;
            ModifiersInBNotInA = bNotinA;
        }
    }
}
