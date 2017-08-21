using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class ChangeType
    {
        public int MissingReactionsFromA { get; set; }

        public int MissingReactionsFromB { get; set; }

        public int DifferentModifiers { get; set; }

    }
}
