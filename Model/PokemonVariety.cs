using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class PokemonVariety
    {
        [JsonProperty(PropertyName = "name")]
        public string VarietyName { get; private set; }

        [JsonIgnore]
        public string ThumbnailUrl
        {
            get { return Sprites.FrontDefaultUrl; }
        }

        [JsonProperty(PropertyName = "sprites")]
        public PokemonSprites Sprites { get; private set; }

        [JsonProperty(PropertyName = "stats")]
        public List<PokemonStatistic> Stats { get; private set; }

        [JsonProperty(PropertyName = "types")]
        public List<PokemonType> Types { get; private set; }
    }
}
