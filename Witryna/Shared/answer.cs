using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Shared
{
    public class answer
    {
        [Key]
        public int id { get; set; }
        public string? text { get; set; }
        public string? image_url { get; set; }
        public List<question_answer> question_answer { get; set; }
    }
}
