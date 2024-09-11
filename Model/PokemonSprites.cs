using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class PokemonSprites
    {
        [JsonProperty(PropertyName = "front_default")]
        public string FrontDefaultUrl { get; private set; }
    }
}
