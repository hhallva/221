using GameStore.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models
{
    public class Game : ViewModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public virtual IEnumerable<Key> Keys { get; set; }
        public string? ImagePath { get; set; }

        public int KeyCount { get => Keys.Where(x=>!x.IsUsed).Count(); }
    }
}
