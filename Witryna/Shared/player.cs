using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Shared
{
    public class player
    {
        [Key]
        public int id { get; set; }
        public string login { get; set; }
        public byte[] password_hash { get; set; }
        public byte[] password_salt { get; set; }
        public DateTime registered_on { get; set; }
        public List<land> land { get; set; }
        public List<answered_question> answered_question { get; set; }
    }
}