using System;
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

            var reactionsFromModelA = SBMLParser.GetReactions(fileStreamA, compartmentCatalouge);
            var reactionsFromModelB = SBMLParser.GetReactions(fileStreamB, compartmentCatalouge);
            var totalReactionsModelA = reactionsFromModelA.Count;
            var totalReactionsModelB = reactionsFromModelB.Count;

            var stats = new ComparedStatistics();
            var diff = GetDifferences(reactionsFromModelA, reactionsFromModelB, ref stats);
            var models = GetModelDicitionary(fileStreamA, originalPathA, fileStreamB, originalPathB, totalReactionsModelA, totalReactionsModelB, stats);

            var modelComparisonResult = new ModelComparisonResult(diff, stats, models);

            return modelComparisonResult;
        }

        private static Dictionary<string, Model> GetModelDicitionary(FileStream fileStreamA, string originalPathA, FileStream fileStreamB, string originalPathB, int totalReactionsModelA, int totalReactionsModelB, ComparedStatistics stats)
        {
            var modelA = GetModel(fileStreamA, originalPathA, totalReactionsModelA, stats);
            var modelB = GetModel(fileStreamB, originalPathB, totalReactionsModelB, stats);
            var models = new Dictionary<string, Model> { { "A", modelA }, { "B", modelB } };
            return models;
        }

        private static Model GetModel(FileStream fileStream, string originalPath, int totalReactions, ComparedStatistics stats)
        {
            var model = SBMLParser.GetModelDetails(fileStream, originalPath);
            model.TotalReactions = totalReactions;
            model.PercentSharedIdenticalReactions = ((float) stats.SharedReactions / model.TotalReactions) * 100;
            model.PercentSharedReactions = (((float) stats.SharedReactions + stats.DifferentModifiers) / model.TotalReactions) * 100;
            return model;
        }

        private static List<AffectedReaction> GetDifferences(List<Reaction> modelA, List<Reaction> modelB, ref ComparedStatistics stats)
        {
            var diff = new List<AffectedReaction>();
            foreach (var aReaction in modelA)
            {
                var bReaction = modelB.FirstOrDefault(r => r.ReactionId == aReaction.ReactionId);
                if (bReaction != null)
                {
                    AddLowPriorityDifferece(aReaction, bReaction, ref diff, ref stats);
                    modelB.Remove(bReaction);
                }
                else
                    AddHighPriorityDifferenceAOnly(diff, stats, aReaction);
            }

            foreach (var bReaction in modelB)
            {
                var aReaction = modelA.FirstOrDefault(r => r.ReactionId == bReaction.ReactionId);
                if (aReaction != null)
                    AddLowPriorityDifferece(aReaction, bReaction, ref diff, ref stats);
                else
                    AddHighPriorityDifferenceBOnly(diff, stats, bReaction);
            }

            return diff;
        }

        private static void AddLowPriorityDifferece(Reaction aReaction, Reaction bReaction, ref List<AffectedReaction> diff, ref ComparedStatistics stats)
        {
            var aMissingInB = aReaction.Modifiers.Except(bReaction.Modifiers).ToList();
            var bMissingInA = bReaction.Modifiers.Except(aReaction.Modifiers).ToList();
            var same = !aMissingInB.Any() && !bMissingInA.Any();
            if (!same)
            {
                stats.RecordDifferentModifiers(aReaction.Subsystem, aReaction.Compartments);
                var newComparedReaction = new AffectedModifiedReaction
                {
                    ReactionId = aReaction.ReactionId,
                    Compartments = aReaction.Compartments,
                    Subsystem = aReaction.Subsystem,
                    ModifierDifferences = new ModifierDifferences(aMissingInB, bMissingInA)
                };
                diff.Add(newComparedReaction);
            }
            else
                stats.RecordShareReactions();
        }

        private static void AddHighPriorityDifferenceAOnly(List<AffectedReaction> diff, ComparedStatistics stats, Reaction aReaction)
        {
            stats.RecordReactionOnlyInA(aReaction.Subsystem, aReaction.Compartments);
            var newComparedReaction = new AffectedLostReaction
            {
                ReactionId = aReaction.ReactionId,
                Compartments = aReaction.Compartments,
                FoundInA = true,
                FoundInB = false,
                Subsystem = aReaction.Subsystem,
            };
            diff.Add(newComparedReaction);
        }

        private static void AddHighPriorityDifferenceBOnly(List<AffectedReaction> diff, ComparedStatistics stats, Reaction bReaction)
        {
            stats.RecordReactionOnlyInB(bReaction.Subsystem, bReaction.Compartments);
            var newComparedReaction = new AffectedLostReaction
            {
                ReactionId = bReaction.ReactionId,
                Compartments = bReaction.Compartments,
                FoundInA = false,
                FoundInB = true,
                Subsystem = bReaction.Subsystem,
            };
            diff.Add(newComparedReaction);
        }
    }
}
