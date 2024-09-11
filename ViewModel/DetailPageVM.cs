using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using PokeCollection.Model;
using PokeCollection.Repository;

namespace PokeCollection.ViewModel
{
    public class DetailPageVM : ObservableObject
    {
        private PokemonSpecies _currentPokemon = null;
        public PokemonSpecies CurrentPokemon
        {
            get { return _currentPokemon; }
            set
            {
                _currentPokemon = value;
                OnPropertyChanged(nameof(CurrentPokemon));
            }
        }

        private EvolutionChain _evolutionChain = null;
        public EvolutionChain EvolutionChain
        {
            get { return _evolutionChain; }
            set
            {
                _evolutionChain = value;
                OnPropertyChanged(nameof(EvolutionChain));
            }
        }
    }
}
