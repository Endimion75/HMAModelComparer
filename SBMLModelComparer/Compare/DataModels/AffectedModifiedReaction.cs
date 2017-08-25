using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class AffectedModifiedReaction : AffectedReaction
    {
        public ModifierDifferences ModifierDiferenceses { get; set; }
        public AffectedModifiedReaction()
        {
            ModifierDiferenceses = new ModifierDifferences();
        }
    }
}
