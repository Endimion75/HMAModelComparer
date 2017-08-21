using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    enum TypeOfChange
    {
        MissingReactionsFromA,
        MissingReactionsFromB,
        DifferentModifiers
    }
    public class ComparedStatistics
    {
        public Dictionary<string, ChangeType> AffectedSubsystems { get; set; }

        public Dictionary<string, ChangeType> AffectedCompartments { get; set; }

        public int SharedReactions { get; set; }

        public int ReactionsOnlyInA { get; set; }

        public int ReactionsOnlyInB { get; set; }

        public ComparedStatistics()
        {
            AffectedCompartments = new Dictionary<string, ChangeType>();
            AffectedSubsystems = new Dictionary<string, ChangeType>();
            ReactionsOnlyInA = 0;
            ReactionsOnlyInB = 0;
            SharedReactions = 0;
        }

        public void RecordShareReaction(string subsystem, List<string> compartments)
        {
            IncreaseSharedReaction();
            AddAffectedsubsystem(subsystem, TypeOfChange.DifferentModifiers);
            AddAffectedCompartment(compartments, TypeOfChange.DifferentModifiers);
        }

        public void RecordReactionOnlyInA(string subsystem, List<string> compartments)
        {
            IncreaseReactionsOnlyInA();
            AddAffectedsubsystem(subsystem, TypeOfChange.MissingReactionsFromB);
            AddAffectedCompartment(compartments, TypeOfChange.MissingReactionsFromB);
        }

        public void RecordReactionOnlyInB(string subsystem, List<string> compartments)
        {
            IncreaseReactionsOnlyInB();
            AddAffectedsubsystem(subsystem, TypeOfChange.MissingReactionsFromA);
            AddAffectedCompartment(compartments, TypeOfChange.MissingReactionsFromA);
        }

        private void AddAffectedsubsystem(string subsystem, TypeOfChange change)
        {
            if (!AffectedSubsystems.ContainsKey(subsystem))
            {
                AffectedSubsystems.Add(subsystem, new ChangeType());
            }
            else
            {
                switch (change)
                {
                    case TypeOfChange.DifferentModifiers:
                        AffectedSubsystems[subsystem].DifferentModifiers += 1;
                        break;
                    case TypeOfChange.MissingReactionsFromA:
                        AffectedSubsystems[subsystem].MissingReactionsFromA += 1;
                        break;
                    case TypeOfChange.MissingReactionsFromB:
                        AffectedSubsystems[subsystem].MissingReactionsFromB += 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(change), change, null);
                }
            }
        }

        private void AddAffectedCompartment(List<string> compartments, TypeOfChange change)
        {
            foreach (var compartment in compartments)
            {
                if (!AffectedCompartments.ContainsKey(compartment))
                {
                    AffectedCompartments.Add(compartment, new ChangeType());
                }
                else
                {
                    switch (change)
                    {
                        case TypeOfChange.DifferentModifiers:
                            AffectedCompartments[compartment].DifferentModifiers += 1;
                            break;
                        case TypeOfChange.MissingReactionsFromA:
                            AffectedCompartments[compartment].MissingReactionsFromA += 1;
                            break;
                        case TypeOfChange.MissingReactionsFromB:
                            AffectedCompartments[compartment].MissingReactionsFromB += 1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(change), change, null);
                    }
                }
            }

           
        }

        private void IncreaseReactionsOnlyInA()
        {
            ReactionsOnlyInA += 1;
        }

        private void IncreaseReactionsOnlyInB()
        {
            ReactionsOnlyInB += 1;
        }

        private void IncreaseSharedReaction()
        {
            SharedReactions += 1;
        }
    }
}
