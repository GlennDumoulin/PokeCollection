using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeCollection.Model
{
    public interface IPokemonRepository
    {
        //Pokemon functions
        Task<List<PokemonSpecies>> GetPokemons(bool canRetry = true);
        Task<List<PokemonSpecies>> GetPokemons(string pokemonType, string pokemonGeneration, string pokemonRarity);

        //Pokemon Types functions
        Task<List<string>> GetPokemonTypes();
        Task<List<PokemonSpecies>> GetPokemonsByType(string pokemonType);

        //Pokemon Generations functions
        Task<List<string>> GetPokemonGenerations();
        Task<List<PokemonSpecies>> GetPokemonsByGeneration(string pokemonGeneration);

        //Pokemon Rarities functions
        Task<List<string>> GetPokemonRarities();
        Task<List<PokemonSpecies>> GetPokemonsByRarity(string pokemonRarity);

        //Evolution Chain functions
        Task<EvolutionChain> GetEvolutionChainById(int chainId);
    }
}
