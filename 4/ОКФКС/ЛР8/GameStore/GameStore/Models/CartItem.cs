using GameStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models
{
    // Models/CartItem.cs
    public class CartItem : ViewModelBase
    {
        private int quantity;

        public Game Game { get; set; }
        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal TotalPrice => Game.Price * Quantity;
    }

}
