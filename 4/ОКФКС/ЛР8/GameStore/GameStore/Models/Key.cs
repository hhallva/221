using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Models
{
    public class Key
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public int GameId { get; set; }
        public virtual Game Game { get; set; }
        public bool IsUsed { get; set; }
    }
}
