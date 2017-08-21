using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Compare.DataModels
{
    public class Model
    {
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string File { get; set; }
        public int TotalReactions { get; set; }

        public Model()
        {
            
        }

        public Model(string file, string modelId, string modelName, int totalReactions)
        {
            File = file;
            ModelId = modelId;
            ModelName = modelName;
            TotalReactions = totalReactions;
        }
    }
}
