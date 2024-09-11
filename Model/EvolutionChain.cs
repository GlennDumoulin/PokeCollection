using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PokeCollection.Model
{
    public class SpeciesInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonIgnore]
        public string ThumbnailUrl { get; private set; } = string.Empty;

        public void SetThumbnailUrl(string thumbnailUrl)
        {
            //Make sure we can only set thumbnail url once
            if (ThumbnailUrl != string.Empty) return;

            ThumbnailUrl = thumbnailUrl;
        }
    }

    public class EvolutionSpecies
    {
        [JsonProperty(PropertyName = "species")]
        public SpeciesInfo Info { get; private set; }

        [JsonProperty(PropertyName = "evolves_to")]
        public List<EvolutionSpecies> Evolutions { get; private set; }
    }

    public class EvolutionChain
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "chain")]
        public EvolutionSpecies Base { get; private set; }
    }

    public class EvolutionChainInfo
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; private set; }

        [JsonIgnore]
        private int _id = 0;
        public int Id
        {
            get { return (_id != 0) ? _id : GetEvolutionChainId(); }
            private set
            {
                _id = value;
            }
        }

        private int GetEvolutionChainId()
        {
            //Check if there is a evolution chain
            if (Url == null) return -1;

            //Get the substring of the evolution chain
            string evoChainString = "evolution-chain/";
            int evoChainIdx = Url.IndexOf(evoChainString);
            if (evoChainIdx == -1) return -1;

            //Get the id of the evolution chain
            string evoChainSubstring = Url.Substring(evoChainIdx + evoChainString.Length);
            string evoChainId = evoChainSubstring.Substring(0, evoChainSubstring.Length - 1);
            Id = Convert.ToInt32(evoChainId);

            return Id;
        }
    }
}
