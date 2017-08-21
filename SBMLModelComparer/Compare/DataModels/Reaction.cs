using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class Reaction
    {
        public string ReactionId { get; set; }

        public string Subsystem { get; set; }

        public List<string> Compartments { get; set; }

        public List<string> Modifiers { get; set; }

        public Reaction()
        {
            Modifiers = new List<string>();
            Compartments = new List<string>();
        }

        public Reaction(string reactionId, string subsystem, List<string> compartments,List<string> modifiers)
        {
            ReactionId = reactionId;
            Subsystem = subsystem;
            Modifiers = modifiers;
            Compartments = compartments;
        }
    }
}
