using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PokeCollection.Model;
using PokeCollection.Repository;
using PokeCollection.View;

namespace PokeCollection.ViewModel
{
    public class OverViewPageVM : ObservableObject
    {
        //Data functionality
        public List<PokemonSpecies> Pokemons { get; private set; } = null;

        private List<string> _pokemonTypes = null;
        public List<string> PokemonTypes
        {
            get { return _pokemonTypes; }
            set
            {
                _pokemonTypes = value;
                OnPropertyChanged(nameof(PokemonTypes));
            }
        }

        private List<string> _pokemonGenerations = null;
        public List<string> PokemonGenerations
        {
            get { return _pokemonGenerations; }
            set
            {
                _pokemonGenerations = value;
                OnPropertyChanged(nameof(PokemonGenerations));
            }
        }

        private List<string> _pokemonRarities = null;
        public List<string> PokemonRarities
        {
            get { return _pokemonRarities; }
            set
            {
                _pokemonRarities = value;
                OnPropertyChanged(nameof(PokemonRarities));
            }
        }

        public List<string> SortOptions { get; private set; } = new List<string>() { "id", "name", "evolution chain" };
        public List<string> SortOrders { get; private set; } = new List<string>() { "ascending", "descending" };

        private PokemonSpecies _selectedPokemon;
        public PokemonSpecies SelectedPokemon
        {
            get { return _selectedPokemon; }
            set
            {
                _selectedPokemon = value;
                OnPropertyChanged(nameof(SelectedPokemon));
            }
        }

        private string _selectedType;
        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));

                SetPokemonsToDisplay();
            }
        }

        private string _selectedGeneration;
        public string SelectedGeneration
        {
            get { return _selectedGeneration; }
            set
            {
                _selectedGeneration = value;
                OnPropertyChanged(nameof(SelectedGeneration));

                SetPokemonsToDisplay();
            }
        }

        private string _selectedRarity;
        public string SelectedRarity
        {
            get { return _selectedRarity; }
            set
            {
                _selectedRarity = value;
                OnPropertyChanged(nameof(SelectedRarity));

                SetPokemonsToDisplay();
            }
        }

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get { return _selectedSortOption; }
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged(nameof(SelectedSortOption));

                SetPokemonsToDisplay();
            }
        }

        private string _selectedSortOrder;
        public string SelectedSortOrder
        {
            get { return _selectedSortOrder; }
            set
            {
                _selectedSortOrder = value;
                OnPropertyChanged(nameof(SelectedSortOrder));

                SetPokemonsToDisplay();
            }
        }

        private async void FilterPokemons()
        {
            Pokemons = await Repository.GetPokemons(SelectedType, SelectedGeneration, SelectedRarity);
        }

        private void SortPokemons()
        {
            if (Pokemons == null) return;

            bool isAscending = SelectedSortOrder == SortOrders[0];

            switch (SelectedSortOption)
            {
                case "id":
                    {
                        Pokemons = ((isAscending) ? Pokemons.OrderBy(p => p.Id) : Pokemons.OrderByDescending(p => p.Id)).ToList();
                        break;
                    }
                case "name":
                    {
                        Pokemons = ((isAscending) ? Pokemons.OrderBy(p => p.Name) : Pokemons.OrderByDescending(p => p.Name)).ToList();
                        break;
                    }
                case "evolution chain":
                    {
                        Pokemons = ((isAscending) ? Pokemons.OrderBy(p => p.EvolutionChain.Id) : Pokemons.OrderByDescending(p => p.EvolutionChain.Id)).ToList();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void SetPokemonsToDisplay()
        {
            FilterPokemons();
            SortPokemons();
            //PaginatePokemons();
            OnPropertyChanged(nameof(Pokemons));
        }

        private void ResetPokemons()
        {
            Pokemons = null;
            OnPropertyChanged(nameof(Pokemons));
        }

        //Repository functionality
        private PokemonLocalRepository LocalRepository { get; } = new PokemonLocalRepository();
        private PokemonApiRepository ApiRepository { get; } = new PokemonApiRepository();

        public IPokemonRepository Repository { get; private set; }

        public string CommandText
        {
            get
            {
                //Check the current visible page type
                if (Repository is PokemonLocalRepository) //Local --> go to API
                {
                    return "USE API DATA";
                }
                else //API --> go to Local
                {
                    return "USE LOCAL DATA";
                }
            }
        }

        public RelayCommand SwitchRepositoryCommand { get; private set; }

        public void SwitchRepository()
        {
            //Check the current repository type
            if (Repository is PokemonLocalRepository) //Local --> go to API
            {
                Repository = ApiRepository;
                OnPropertyChanged(nameof(Repository));
                OnPropertyChanged(nameof(CommandText));

                ResetPokemons();
                ResetPagination();

                InitializeFiltersAndSorting();

                SetPokemonsToDisplay();
            }
            else //API --> go to Local
            {
                Repository = LocalRepository;
                OnPropertyChanged(nameof(Repository));
                OnPropertyChanged(nameof(CommandText));

                ResetPokemons();
                ResetPagination();

                InitializeFiltersAndSorting();

                SetPokemonsToDisplay();
            }
        }

        //Pagination functionality
        private int ItemsPerPage { get; } = 50;
        private int PageIndex { get; set; } = 0;
        private bool IsFirstPage { get; set; } = true;
        private bool IsLastPage { get; set; } = true;

        public bool CanDoNextPage
        {
            get { return !IsLastPage; }
        }
        public bool CanDoPrevPage
        {
            get { return !IsFirstPage; }
        }

        private void PaginatePokemons()
        {
            if (Pokemons == null) return;

            int startIdx = PageIndex * ItemsPerPage;
            int amountRemaining = Pokemons.Count - startIdx;
            int range = (amountRemaining > ItemsPerPage) ? ItemsPerPage : amountRemaining;

            Pokemons = Pokemons.GetRange(startIdx, range);
        }

        private void ResetPagination()
        {
            PageIndex = 0;
            OnPropertyChanged(nameof(PageIndex));
            IsFirstPage = true;
            OnPropertyChanged(nameof(IsFirstPage));
            IsLastPage = true;
            OnPropertyChanged(nameof(IsLastPage));
        }

        public RelayCommand NextPageCommand { get; private set; }
        public void NextPage()
        {
            if (Pokemons == null) return;

            //Ignore if we are already on the last page
            if (IsLastPage) return;

            //Handle navigating to next page
            ++PageIndex;
            OnPropertyChanged(nameof(PageIndex));

            PaginatePokemons();

            IsFirstPage = false;
            OnPropertyChanged(nameof(IsFirstPage));

            int nrOfPages = (Pokemons.Count - 1) / ItemsPerPage;
            if (PageIndex == nrOfPages)
            {
                IsLastPage = true;
                OnPropertyChanged(nameof(IsLastPage));
            }
        }

        public RelayCommand PrevPageCommand { get; private set; }
        public void PrevPage()
        {
            if (Pokemons == null) return;

            //Ignore if we are already on the first page
            if (IsFirstPage) return;

            //Handle navigating to previous page
            --PageIndex;
            OnPropertyChanged(nameof(PageIndex));

            PaginatePokemons();

            IsLastPage = false;
            OnPropertyChanged(nameof(IsLastPage));

            if (PageIndex == 0)
            {
                IsFirstPage = true;
                OnPropertyChanged(nameof(IsFirstPage));
            }
        }

        //Initialization functionality
        public OverViewPageVM()
        {
            Repository = ApiRepository;
            SwitchRepositoryCommand = new RelayCommand(SwitchRepository);

            NextPageCommand = new RelayCommand(NextPage);
            PrevPageCommand = new RelayCommand(PrevPage);

            InitializeFiltersAndSorting();
        }

        private async void InitializeFiltersAndSorting()
        {
            List<Task<List<string>>> parallelTasks = new List<Task<List<string>>>
            {
                Repository.GetPokemonTypes(),
                Repository.GetPokemonGenerations(),
                Repository.GetPokemonRarities()
            };
            var results = await Task.WhenAll(parallelTasks);

            PokemonTypes = results[0];
            PokemonGenerations = results[1];
            PokemonRarities = results[2];

            SelectedType = PokemonTypes[0];
            SelectedGeneration = PokemonGenerations[0];
            SelectedRarity = PokemonRarities[0];

            SelectedSortOption = SortOptions[0];
            SelectedSortOrder = SortOrders[0];
        }
    }
}
