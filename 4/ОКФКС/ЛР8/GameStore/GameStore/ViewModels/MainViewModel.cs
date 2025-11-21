using GameStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace GameStore.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<CartItem> CartItems { get; set; }
        public decimal TotalPrice => CartItems.Sum(game => game.TotalPrice);

        public decimal Balance { get; set; } = 10_000;

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand OrderCommand { get; }

        public MainViewModel()
        {
            using var context = new AppDbContext();
            Games = new ObservableCollection<Game>(context.Games.Include(g => g.Keys).ToList());

            CartItems = new ObservableCollection<CartItem>();
            AddToCartCommand = new RelayCommand<Game>(AddToCart);
            RemoveFromCartCommand = new RelayCommand<Game>(RemoveFromCart);
            CheckoutCommand = new RelayCommand<object>(Checkout);
        }

        public void AddToCart(Game game)
        {
            var cartItem = CartItems.FirstOrDefault(item => item.Game.Id == game.Id);
            if (cartItem == null)
            {
                CartItems.Add(new CartItem { Game = game, Quantity = 1 });
            }
            else
            {
                cartItem.Quantity++;
            }

            OnPropertyChanged(nameof(TotalPrice));
        }

        public void RemoveFromCart(Game game)
        {
            var cartItem = CartItems.FirstOrDefault(item => item.Game.Id == game.Id);
            if (cartItem != null)
            {
                cartItem.Quantity--;
            }

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void Checkout(object o)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Спасибо за покупку! Ваши ключи:");
            using var context = new AppDbContext();

            foreach (var item in CartItems)
            {
                var result = item.Game.Keys.Where(x => !x.IsUsed).Take(item.Quantity).ToList();
                sb.AppendLine(item.Game.Title);
                result.ForEach(key => sb.AppendLine(key.Code));
                result.ForEach(key => key.IsUsed = true);
                context.UpdateRange(result);
            }

            context.SaveChanges();

            OnSendMessage(sb.ToString());
            Balance -= TotalPrice;
            OnPropertyChanged(nameof(Balance));
            CartItems.Clear();
            OnPropertyChanged(nameof(TotalPrice));
        }
    }
}
