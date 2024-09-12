using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PokeCollection.Model;

namespace PokeCollection.Repository
{
    public class PokemonApiRepository : IPokemonRepository
    {
        private static int NrOfPokemonsToLoad { get; } = int.MaxValue/*50*/;
        private static int StartLoadingAtIdx { get; } = 0;

        //Collections data
        private static List<PokemonSpecies> Pokemons { get; set; } = null;
        private static List<EvolutionChain> EvolutionChains { get; set; } = null;

        //Distinct data (mostly for filters)
        private static List<string> PokemonTypes { get; set; } = null;
        private static List<string> PokemonGenerations { get; set; } = null;
        private static List<string> PokemonRarities { get; set; } = null;

        //Pokemon functions
        public async Task<List<PokemonSpecies>> GetPokemons(bool canRetry = true)
        {
            //Reset already loaded pokemon when retrying
            if (canRetry == false) Pokemons = null;

            //Make sure we only need to read the pokemons once
            if (Pokemons != null) return Pokemons;

            string endPoint = $"https://pokeapi.co/api/v2/pokemon-species?limit={NrOfPokemonsToLoad}&offset={StartLoadingAtIdx}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(endPoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }
                    string json = await response.Content.ReadAsStringAsync();

                    List<JToken> pokemonData = JToken.Parse(json).SelectToken("results").ToList();

                    List<Task<PokemonSpecies>> parallelTasks = new List<Task<PokemonSpecies>>();
                    foreach (JToken pokemon in pokemonData)
                    {
                        string pokemonName = pokemon.Value<string>("name");

                        parallelTasks.Add(GetPokemonByName(pokemonName));
                    }

                    var results = await Task.WhenAll(parallelTasks);
                    Pokemons = results.ToList();

                    return Pokemons;
                }
                catch
                {
                    return (canRetry ? await GetPokemons(false) : null);
                }
            }
        }

        public async Task<List<PokemonSpecies>> GetPokemons(string pokemonType, string pokemonGeneration, string pokemonRarity)
        {
            if (Pokemons == null) return null;

            List<Task<List<PokemonSpecies>>> parallelTasks = new List<Task<List<PokemonSpecies>>>
            {
                GetPokemonsByType(pokemonType),
                GetPokemonsByGeneration(pokemonGeneration),
                GetPokemonsByRarity(pokemonRarity)
            };
            var results = await Task.WhenAll(parallelTasks);

            return results[0].Intersect(results[1].Intersect(results[2])).ToList();
        }

        private async Task<PokemonSpecies> GetPokemonByName(string name)
        {
            if (Pokemons != null)
            {
                PokemonSpecies pokemonSpecies = Pokemons.Where(p => p.Name.Equals(name)).FirstOrDefault();
                if (pokemonSpecies == null)
                {
                    return await GetPokemonSpecies(name);
                }
                else
                {
                    return pokemonSpecies;
                }
            }
            else
            {
                return await GetPokemonSpecies(name);
            }
        }

        private async Task<PokemonSpecies> GetPokemonSpecies(string name, bool canRetry = true)
        {
            string endPoint = $"https://pokeapi.co/api/v2/pokemon-species/{name}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(endPoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }
                    string json = await response.Content.ReadAsStringAsync();

                    PokemonSpecies species = JsonConvert.DeserializeObject<PokemonSpecies>(json);
                    List<JToken> varietiesData = JToken.Parse(json).SelectToken("varieties").ToList();

                    List<Task<PokemonVariety>> parallelTasks = new List<Task<PokemonVariety>>();
                    foreach (JToken variety in varietiesData)
                    {
                        string varietyName = variety.Value<JToken>("pokemon").Value<string>("name");

                        parallelTasks.Add(GetPokemonVariety(varietyName));
                    }

                    var results = await Task.WhenAll(parallelTasks);
                    species.SetVarieties(results.ToList());

                    return species;
                }
                catch
                {
                    return (canRetry ? await GetPokemonSpecies(name, false) : null);
                }
            }
        }

        private async Task<PokemonVariety> GetPokemonVariety(string name, bool canRetry = true)
        {
            string endPoint = $"https://pokeapi.co/api/v2/pokemon/{name}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(endPoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }
                    string json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<PokemonVariety>(json);
                }
                catch
                {
                    return (canRetry ? await GetPokemonVariety(name, false) : null);
                }
            }
        }

        //Pokemon Types functions
        public async Task<List<string>> GetPokemonTypes()
        {
            //Make sure we only need to read the pokemon types once
            if (PokemonTypes != null) return PokemonTypes;

            List<PokemonSpecies> pokemons = await GetPokemons();

            List<string> pokemonTypes = new List<string>
            {
                "<all types>"
            };
            foreach (PokemonSpecies pokemon in pokemons)
            {
                PokemonVariety mainVariaty = pokemon.Varieties[0];

                pokemonTypes.AddRange(mainVariaty.Types.Select(t => t.Info.Name));
            }
            PokemonTypes = pokemonTypes.Distinct().ToList();

            return PokemonTypes;
        }

        public async Task<List<PokemonSpecies>> GetPokemonsByType(string pokemonType)
        {
            List<PokemonSpecies> pokemons = await GetPokemons();

            //Check default filter
            List<string> pokemonTypes = await GetPokemonTypes();
            if (pokemonType == pokemonTypes[0]) return pokemons;

            //Filter based on type
            return pokemons.Where(p => p.Varieties[0].Types.Any(t => t.Info.Name.Equals(pokemonType))).ToList();
        }

        //Pokemon Generations functions
        public async Task<List<string>> GetPokemonGenerations()
        {
            //Make sure we only need to read the pokemon generations once
            if (PokemonGenerations != null) return PokemonGenerations;

            List<PokemonSpecies> pokemons = await GetPokemons();

            List<string> pokemonGenerations = new List<string>
            {
                "<all generations>"
            };
            pokemonGenerations.AddRange(pokemons.Select(p => p.Generation.Name).Distinct());
            PokemonGenerations = pokemonGenerations;

            return PokemonGenerations;
        }

        public async Task<List<PokemonSpecies>> GetPokemonsByGeneration(string pokemonGeneration)
        {
            List<PokemonSpecies> pokemons = await GetPokemons();

            //Check default filter
            List<string> pokemonGenerations = await GetPokemonGenerations();
            if (pokemonGeneration == pokemonGenerations[0]) return pokemons;

            //Filter based on generation
            return pokemons.Where(p => p.Generation.Name.Equals(pokemonGeneration)).ToList();
        }

        //Pokemon Rarities functions
        public async Task<List<string>> GetPokemonRarities()
        {
            //Make sure we only need to read the pokemon generations once
            if (PokemonRarities != null) return PokemonRarities;

            List<PokemonSpecies> pokemons = await GetPokemons();

            List<string> pokemonRarities = new List<string>
            {
                "<all rarities>"
            };
            if (pokemons.Any(p => p.IsRegular)) pokemonRarities.Add("regular");
            if (pokemons.Any(p => p.IsBaby)) pokemonRarities.Add("baby");
            if (pokemons.Any(p => p.IsLegendary)) pokemonRarities.Add("legendary");
            if (pokemons.Any(p => p.IsMythical)) pokemonRarities.Add("mythical");
            PokemonRarities = pokemonRarities;

            return PokemonRarities;
        }

        public async Task<List<PokemonSpecies>> GetPokemonsByRarity(string pokemonRarity)
        {
            List<PokemonSpecies> pokemons = await GetPokemons();

            //Check default filter
            List<string> pokemonRarities = await GetPokemonRarities();
            if (pokemonRarity == pokemonRarities[0]) return pokemons;

            //Filter based on rarity
            switch (pokemonRarity)
            {
                case "regular":
                    return pokemons.Where(p => p.IsRegular).ToList();
                case "baby":
                    return pokemons.Where(p => p.IsBaby).ToList();
                case "legendary":
                    return pokemons.Where(p => p.IsLegendary).ToList();
                case "mythical":
                    return pokemons.Where(p => p.IsMythical).ToList();
                default:
                    return pokemons;
            }
        }

        //Evolution Chain functions
        public async Task<EvolutionChain> GetEvolutionChainById(int chainId)
        {
            if (EvolutionChains != null)
            {
                EvolutionChain evolutionChain = EvolutionChains.Where(e => e.Id == chainId).FirstOrDefault();
                if (evolutionChain == null)
                {
                    return await GetEvolutionChain(chainId);
                }
                else
                {
                    return evolutionChain;
                }
            }
            else
            {
                return await GetEvolutionChain(chainId);
            }
        }

        private async Task<EvolutionChain> GetEvolutionChain(int chainId, bool canRetry = true)
        {
            string endPoint = $"https://pokeapi.co/api/v2/evolution-chain/{chainId}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(endPoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }
                    string json = await response.Content.ReadAsStringAsync();

                    EvolutionChain evolutionChain = JsonConvert.DeserializeObject<EvolutionChain>(json);

                    List<PokemonSpecies> pokemons = await GetPokemons();

                    //Get the base pokemon
                    PokemonSpecies basePokemon = pokemons.Where(p => p.Name.Equals(evolutionChain.Base.Info.Name)).FirstOrDefault();
                    if (basePokemon == null)
                    {
                        basePokemon = await GetPokemonSpecies(evolutionChain.Base.Info.Name);
                    }

                    //Set thumbnail url for base pokemon
                    string baseThumbnailUrl = basePokemon.MainThumbnailUrl;
                    evolutionChain.Base.Info.SetThumbnailUrl(baseThumbnailUrl);

                    //Set thumbnail urls for evolutions
                    await SetEvolutionThumbnailUrls(evolutionChain.Base.Evolutions, pokemons);

                    //Store the evolution chain
                    if (EvolutionChains == null)
                    {
                        EvolutionChains = new List<EvolutionChain>() { evolutionChain };
                    }
                    else
                    {
                        EvolutionChains.Add(evolutionChain);
                    }

                    return evolutionChain;
                }
                catch
                {
                    return (canRetry ? await GetEvolutionChain(chainId, false) : null);
                }
            }
        }

        private async Task SetEvolutionThumbnailUrls(List<EvolutionSpecies> evolutionSpecies, List<PokemonSpecies> pokemons)
        {
            //Check if there are species in the list
            if (evolutionSpecies == null || evolutionSpecies.Count == 0) return;

            foreach (EvolutionSpecies species in evolutionSpecies)
            {
                //Get the evolution pokemon
                PokemonSpecies evolutionPokemon = pokemons.Where(p => p.Name.Equals(species.Info.Name)).FirstOrDefault();
                if (evolutionPokemon == null)
                {
                    evolutionPokemon = await GetPokemonSpecies(species.Info.Name);
                }

                //Set thumbnail url for evolution pokemon
                string baseThumbnailUrl = evolutionPokemon.MainThumbnailUrl;
                species.Info.SetThumbnailUrl(baseThumbnailUrl);

                //Set thumbnail urls for evolutions
                await SetEvolutionThumbnailUrls(species.Evolutions, pokemons);
            }
        }
    }
}
