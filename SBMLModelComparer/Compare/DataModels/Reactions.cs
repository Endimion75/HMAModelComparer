using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class Reactions
    {
        public List<AffectedLostReaction> LostReactions { get; set; }
        public List<AffectedModifiedReaction> VariousModifiers { get; set; }

        public Reactions()
        {
            LostReactions = new List<AffectedLostReaction>();
            VariousModifiers = new List<AffectedModifiedReaction>();
        }
    }
}
