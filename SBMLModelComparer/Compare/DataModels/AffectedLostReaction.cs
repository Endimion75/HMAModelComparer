using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class AffectedLostReaction : AffectedReaction
    {
        public bool FoundInA { get; set; }

        public bool FoundInB { get; set; }

    }
}
