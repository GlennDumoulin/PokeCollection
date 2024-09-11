using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class PokemonSpecies
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonIgnore]
        public string MainThumbnailUrl
        {
            get { return Varieties[0].ThumbnailUrl; }
        }

        [JsonIgnore]
        public string MainType
        {
            get
            {
                return Varieties[0].Types[0].Info.Name;
            }
        }

        [JsonIgnore]
        public List<PokemonStatistic> MainStats
        {
            get
            {
                return Varieties[0].Stats;
            }
        }

        [JsonProperty(PropertyName = "is_baby")]
        public bool IsBaby { get; private set; }

        [JsonProperty(PropertyName = "is_legendary")]
        public bool IsLegendary { get; private set; }

        [JsonProperty(PropertyName = "is_mythical")]
        public bool IsMythical { get; private set; }

        [JsonIgnore]
        public bool IsRegular
        {
            get { return !(IsBaby || IsLegendary || IsMythical); }
        }

        [JsonProperty(PropertyName = "generation")]
        public PokemonGeneration Generation { get; private set; }

        [JsonProperty(PropertyName = "evolution_chain")]
        public EvolutionChainInfo EvolutionChain { get; private set; }

        [JsonIgnore]
        public List<PokemonVariety> Varieties { get; private set; } = null;

        public void SetVarieties(List<PokemonVariety> varieties)
        {
            //Make sure we can only set varieties once
            if (Varieties != null) return;

            Varieties = varieties;
        }
    }
}
