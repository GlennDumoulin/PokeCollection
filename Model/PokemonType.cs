using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class TypeInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
    }

    public class PokemonType
    {
        [JsonProperty(PropertyName = "slot")]
        public int Slot { get; private set; }

        [JsonProperty(PropertyName = "type")]
        public TypeInfo Info { get; private set; }
    }
}
