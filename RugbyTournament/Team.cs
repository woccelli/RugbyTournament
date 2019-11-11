using System;
using System.Collections.Generic;
using System.Text;

namespace RugbyTournament
{
    class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Team(string name, int id)
        {
            this.Name = name; 
            this.Id = id;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
