using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Shared
{
    public class land
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public int? player_id { get; set; }
        [ForeignKey("player_id")]
        public player? player { get; set; }
        public int realm_id { get; set; }
        [ForeignKey("realm_id")]
        public realm realm { get; set; }
    }
}