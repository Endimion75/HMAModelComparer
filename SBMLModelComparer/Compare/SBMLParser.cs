using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Compare.Classes;
using Compare.DataModels;

namespace Compare
{
    public static class SBMLParser
    {
        public static List<Reaction> CalculateStats(FileStream fileStream, List<Compartment> compartmentCatalogue)
        {
            var doc = XDocument.Load(fileStream);

            var xDocListOfReactionsSubsystem = doc.Descendants().Where(p => p.Name.LocalName == "listOfReactions");
            var xDocRections = xDocListOfReactionsSubsystem.Descendants().Where(p => p.Name.LocalName == "reaction");

            var reactions = new List<Reaction>();

            foreach (var xDocReaction in xDocRections)
            {
                var subsystem = xDocReaction.Descendants().Where(p => p.Name.LocalName == "body").Descendants().Where(p => p.Name.LocalName == "p").Nodes().First().ToString();
                subsystem = subsystem.Replace("SUBSYSTEM:", "").Trim();
                var id = xDocReaction.Attribute("id")?.Value;
                var compartments = GetCompartments(xDocReaction, compartmentCatalogue);
                var modifiers = GetModifiers(xDocReaction);
                var newReaction = new Reaction(id, subsystem, compartments, modifiers);
                reactions.Add(newReaction);
            }
            return reactions;
        }

        public static Model GetModelDetails(FileStream fileStream, string originalPath)
        {
            fileStream.Position = 0;
            var doc = XDocument.Load(fileStream);
            var model = doc.Descendants().Where(p => p.Name.LocalName == "model").ToList();
            var modelName = model.First().Attribute("name")?.Value;
            var id = model.First().Attribute("id")?.Value;
            var newModel = new Model(originalPath, id, modelName, 0);
            return newModel;
        }

        public static List<Compartment> GetCompartmentCatalouge(FileStream fileSteam)
        {
            var doc = XDocument.Load(fileSteam);
            var compartmentCatalogue = GetCompartmentCatalogue(doc);

            return compartmentCatalogue;
        }

        private static List<string> GetModifiers(XElement xDocReaction)
        {
            var listOfModifiers = xDocReaction.Descendants().Where(p => p.Name.LocalName == "listOfModifiers");
            var xDocModifiers = listOfModifiers.Descendants().Where(p => p.Name.LocalName == "modifierSpeciesReference");
            var modifiers = new List<string>();
            foreach (var modifier in xDocModifiers)
            {
                var species = modifier.Attribute("species")?.Value;
                modifiers.Add(species);
            }
            return modifiers;
        }

        private static List<string> GetCompartments(XElement xDocReaction, List<Compartment> compartmentCatalouge)
        {
            var compartments = new List<string>();

            var listOfReactants = xDocReaction.Descendants().Where(p => p.Name.LocalName == "listOfReactants");
            var xDocReactants = listOfReactants.Descendants().Where(p => p.Name.LocalName == "speciesReference");
            foreach (var reactant in xDocReactants)
            {
                var species = reactant.Attribute("species")?.Value;
                if (!Regex.IsMatch(species, "M_m\\d{5}[a-z]{1}"))
                    continue;
                var compartment = compartmentCatalouge.FirstOrDefault(c => c.Id == "C_" + species.GetLast(1));
                if(!compartments.Contains(compartment.Name))
                    compartments.Add(compartment.Name);
            }

            var listOfProducts = xDocReaction.Descendants().Where(p => p.Name.LocalName == "listOfReactants");
            var xDocProducts = listOfProducts.Descendants().Where(p => p.Name.LocalName == "speciesReference");
            foreach (var product in xDocProducts)
            {
                var species = product.Attribute("species")?.Value;
                if (!Regex.IsMatch(species, "M_m\\d{5}[a-z]{1}"))
                    continue;
                var compartment = compartmentCatalouge.FirstOrDefault(c => c.Id == "C_" + species.GetLast(1));
                if (!compartments.Contains(compartment.Name))
                    compartments.Add(compartment.Name);
            }
            return compartments;
        }

        private static List<Compartment> GetCompartmentCatalogue(XDocument doc)
        {
            var listOfSpeciesCompartments = doc.Descendants().Where(p => p.Name.LocalName == "listOfCompartments");
            var compartments = listOfSpeciesCompartments.Descendants().Where(p => p.Name.LocalName == "compartment");
            var compartmentcatalogue = new List<Compartment>();
            foreach (var compartment in compartments)
            {
                var name = compartment.Attribute("name")?.Value;
                var id = compartment.Attribute("id")?.Value;
                compartmentcatalogue.Add(new Compartment(id, name));
            }
            return compartmentcatalogue;
        }
    }

}
