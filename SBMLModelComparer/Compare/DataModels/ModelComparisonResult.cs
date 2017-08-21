using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Compare.DataModels
{
    public class ModelComparisonResult
    {
        public Dictionary<string, Model> Models { get; set; }

        public ComparedStatistics Summary { get; set; }
        
        public Dictionary<string, List<AffectedReaction>> AffectedReactions { get; set; }
        
        public ModelComparisonResult(List<AffectedReaction> reactions, ComparedStatistics statistics, Dictionary<string, Model> models)
        {
            Summary = statistics;
            Models = models;
            AffectedReactions = new Dictionary<string, List<AffectedReaction>>
            {
                {"LostReactions", new List<AffectedReaction>()},
                { "VariousModifiers", new List<AffectedReaction>()}
            };
            foreach (var reaction in reactions)
            {
                if (reaction.Importance == "low")
                    AffectedReactions["VariousModifiers"].Add(reaction);
                else if(reaction.Importance == "high")
                    AffectedReactions["LostReactions"].Add(reaction);
            }
        }

        public ModelComparisonResult()
        {
            AffectedReactions = new Dictionary<string, List<AffectedReaction>>();
            Summary = new ComparedStatistics();
            Models = new Dictionary<string, Model>();
        }
    }
}
