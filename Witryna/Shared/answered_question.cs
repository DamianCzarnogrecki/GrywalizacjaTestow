using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Shared
{
    public class answered_question
    {
        [Key]
        public int id { get; set; }
        public DateTime answered_on { get; set; }
        public int seconds_spent { get; set; }
        public int player_id { get; set; }
        [ForeignKey("player_id")]
        public player player { get; set; }
        public int question_id { get; set; }
        [ForeignKey("question_id")]
        public question question { get; set; }
        public List<given_answer> given_answer { get; set; }
    }
}
