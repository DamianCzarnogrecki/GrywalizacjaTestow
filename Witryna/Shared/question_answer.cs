using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Shared
{
    public class question_answer
    {
        [Key]
        public int id { get; set; }
        public bool is_correct { get; set; }
        public int question_id { get; set; }
        [ForeignKey("question_id")]
        public question question { get; set; }
        public int answer_id { get; set; }
        [ForeignKey("answer_id")]
        public answer answer { get; set; }
        public List<given_answer> given_answer { get; set; }
    }
}