using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public abstract class AffectedReaction
    {
        public string ReactionId { get; set; }

        public string Subsystem { get; set; }

        public List<string> Compartments { get; set; }

    }
}
