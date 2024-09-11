using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class StatInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
    }

    public class PokemonStatistic
    {
        [JsonProperty(PropertyName = "base_stat")]
        public int Value { get; private set; }

        [JsonProperty(PropertyName = "stat")]
        public StatInfo Info { get; private set; }
    }
}
