using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Shared
{
    public class realm
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public List<land> land { get; set; }
    }
}
