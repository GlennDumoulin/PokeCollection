using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class PokemonGeneration
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
    }
}
