using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Shared
{
    public class given_answer
    {
        [Key]
        public int id { get; set; }
        public int answered_question_id { get; set; }
        [ForeignKey("answered_question_id")]
        public answered_question answered_question { get; set; }
        public int question_answer_id { get; set; }
        [ForeignKey("question_answer_id")]
        public question_answer question_answer { get; set; }
    }
}