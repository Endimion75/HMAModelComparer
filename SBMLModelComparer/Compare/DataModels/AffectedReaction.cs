using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class AffectedReaction
    {
        public string ReactionId { get; set; }

        public string Subsystem { get; set; }

        public List<string> Compartments { get; set; }

        public string Importance { get; set; }

        public bool FoundInA { get; set; }

        public bool FoundInB { get; set; }

        public ModifierDifferences ModifierDiferenceses { get; set; }

        public AffectedReaction()
        {
            ModifierDiferenceses = new ModifierDifferences();
        }

    }
}
