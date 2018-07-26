using OrientDB.Net.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class Person : OrientDBEntity
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<string> FavoriteColors { get; set; }

        public override void Hydrate(IDictionary<string, object> data)
        {
            Age = (int)data?["Age"];
            FirstName = data?["FirstName"]?.ToString();
            LastName = data?["LastName"]?.ToString();
            FavoriteColors = data.ContainsKey("FavoriteColors") ? (data?["FavoriteColors"] as IList<object>).Select(n => n.ToString()).ToList() : new List<string>();
        }
    }
}
