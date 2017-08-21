using System;
using System.Collections.Generic;
using System.Text;

namespace Compare.DataModels
{
    public class Compartment
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Compartment()
        {

        }

        public Compartment(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
