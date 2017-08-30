using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class AffectedModifiedReaction : AffectedReaction
    {
        public ModifierDifferences ModifierDifferences { get; set; }
        public AffectedModifiedReaction()
        {
            ModifierDifferences = new ModifierDifferences();
        }
    }
}
