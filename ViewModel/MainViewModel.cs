using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PokeCollection.Model;
using PokeCollection.View;

namespace PokeCollection.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string CommandText
        {
            get
            {
                //Check the current visible page type
                if (CurrentPage is OverViewPage) //Overview page --> go to Details page
                {
                    return "INSPECT POKEMON";
                }
                else //Detail page --> go to Overview page
                {
                    return "BACK TO OVERVIEW";
                }
            }
        }

        public OverViewPage MainPage { get; } = new OverViewPage();
        public DetailPage DetailPage { get; } = new DetailPage();

        public Page CurrentPage { get; private set; }

        public RelayCommand SwitchPageCommand { get; private set; }

        public async void SwitchPage()
        {
            //Check the current visible page type
            if (CurrentPage is OverViewPage) //Overview page --> go to Details page
            {
                //Get the selected pokemon
                PokemonSpecies selectedPokemon = (MainPage.DataContext as OverViewPageVM).SelectedPokemon;
                if (selectedPokemon == null) return;

                //Set the currentPokemon of the DetailPageVM to be the selected Pokemon
                (DetailPage.DataContext as DetailPageVM).CurrentPokemon = selectedPokemon;

                //Set the evolution chain of the current pokemon
                IPokemonRepository repository = (MainPage.DataContext as OverViewPageVM).Repository;
                (DetailPage.DataContext as DetailPageVM).EvolutionChain = await repository.GetEvolutionChainById(selectedPokemon.EvolutionChain.Id);

                //Set the current page
                CurrentPage = DetailPage;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CommandText));
            }
            else //Detail page --> go to Overview page
            {
                CurrentPage = MainPage;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CommandText));
            }
        }

        public MainViewModel()
        {
            CurrentPage = MainPage;

            SwitchPageCommand = new RelayCommand(SwitchPage);
        }
    }
}
