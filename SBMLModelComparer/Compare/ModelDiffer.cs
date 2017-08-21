using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compare.DataModels;

namespace Compare
{
    public static class ModelComparer
    {
        public static ModelComparisonResult GetModelDiferences(FileStream fileStreamA, string originalPathA, FileStream fileStreamB, string originalPathB, FileStream baseFileStream, string originalPathBase)
        {
            var compartmentCatalouge = SBMLParser.GetCompartmentCatalouge(baseFileStream);
            var a = SBMLParser.CalculateStats(fileStreamA, compartmentCatalouge);
            var modelA = SBMLParser.GetModelDetails(fileStreamA, originalPathA);
            modelA.TotalReactions = a.Count;
            var b = SBMLParser.CalculateStats(fileStreamB, compartmentCatalouge);
            var modelB = SBMLParser.GetModelDetails(fileStreamB, originalPathB);
            modelB.TotalReactions = b.Count;
            //var modelBackground = SBMLParser.CalculateStats("data\\base.xml", compartmentCatalouge);


            var models = new Dictionary<string, Model> { { "A", modelA }, { "B", modelB } };
            var stats = new ComparedStatistics();
            var diff = new List<AffectedReaction>();

            diff = GetDifferences(a, b, ref stats);

            var combined = new ModelComparisonResult(diff, stats, models);

            //var jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(combined);
            return combined;

        }
        
        private static List<AffectedReaction> GetDifferences(List<Reaction> modelA, List<Reaction> modelB, ref ComparedStatistics stats)
        {
            var diff = new List<AffectedReaction>();
            var sameCount = 0;
            foreach (var aReaction in modelA)
            {
                var bReaction = modelB.FirstOrDefault(r => r.ReactionId == aReaction.ReactionId);
                if (bReaction != null)
                {
                    AddLowPriorityDifferece(aReaction, bReaction, ref diff, ref stats, ref sameCount);
                    modelB.Remove(bReaction);
                }
                else
                    AddHighPriorityDifferenceAOnly(diff, stats, aReaction);
            }

            foreach (var bReaction in modelB)
            {
                var aReaction = modelA.FirstOrDefault(r => r.ReactionId == bReaction.ReactionId);
                if (aReaction != null)
                    AddLowPriorityDifferece(aReaction, bReaction, ref diff, ref stats, ref sameCount);
                else
                    AddHighPriorityDifferenceBOnly(diff, stats, bReaction);
            }

            return diff;
        }

        private static void AddLowPriorityDifferece(Reaction aReaction, Reaction bReaction, ref List<AffectedReaction> diff, ref ComparedStatistics stats, ref int sameCount)
        {
            var aMissingInB = aReaction.Modifiers.Except(bReaction.Modifiers).ToList();
            var bMissingInA = bReaction.Modifiers.Except(aReaction.Modifiers).ToList();
            var same = !aMissingInB.Any() && !bMissingInA.Any();
            if (!same)
            {
                stats.RecordShareReaction(aReaction.Subsystem, aReaction.Compartments);
                var newComparedReaction = new AffectedReaction
                {
                    ReactionId = aReaction.ReactionId,
                    Compartments = aReaction.Compartments,
                    FoundInA = true,
                    FoundInB = true,
                    Importance = "low",
                    Subsystem = aReaction.Subsystem
                };
                newComparedReaction.ModifierDiferenceses = new ModifierDifferences(aMissingInB, bMissingInA);
                diff.Add(newComparedReaction);
            }
            else
                sameCount++;
        }

        private static void AddHighPriorityDifferenceAOnly(List<AffectedReaction> diff, ComparedStatistics stats, Reaction aReaction)
        {
            stats.RecordReactionOnlyInA(aReaction.Subsystem, aReaction.Compartments);
            var newComparedReaction = new AffectedReaction
            {
                ReactionId = aReaction.ReactionId,
                Compartments = aReaction.Compartments,
                FoundInA = true,
                FoundInB = false,
                Importance = "high",
                Subsystem = aReaction.Subsystem,
                ModifierDiferenceses = null
            };
            diff.Add(newComparedReaction);
        }

        private static void AddHighPriorityDifferenceBOnly(List<AffectedReaction> diff, ComparedStatistics stats, Reaction bReaction)
        {
            stats.RecordReactionOnlyInB(bReaction.Subsystem, bReaction.Compartments);
            var newComparedReaction = new AffectedReaction
            {
                ReactionId = bReaction.ReactionId,
                Compartments = bReaction.Compartments,
                FoundInA = false,
                FoundInB = true,
                Importance = "high",
                Subsystem = bReaction.Subsystem,
                ModifierDiferenceses = null
            };
            diff.Add(newComparedReaction);
        }
    }
}
