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
        
        public Reactions AffectedReactions { get; set; }
        
        public ModelComparisonResult(List<AffectedReaction> reactions, ComparedStatistics statistics, Dictionary<string, Model> models)
        {
            Summary = statistics;
            Models = models;
            AffectedReactions = new Reactions();
            foreach (var reaction in reactions)
            {
                if (reaction is AffectedModifiedReaction)
                    AffectedReactions.VariousModifiers.Add((AffectedModifiedReaction) reaction);
                else if(reaction is AffectedLostReaction)
                    AffectedReactions.LostReactions.Add((AffectedLostReaction) reaction);
            }
        }

        public ModelComparisonResult()
        {
            AffectedReactions = new Reactions();
            Summary = new ComparedStatistics();
            Models = new Dictionary<string, Model>();
        }
    }
}
