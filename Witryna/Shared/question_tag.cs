using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Shared
{
    public class question_tag
    {
        [Key]
        public int id { get; set; }
        public int question_id { get; set; }
        [ForeignKey("question_id")]
        public question question { get; set; }
        public int tag_id { get; set; }
        [ForeignKey("tag_id")]
        public tag tag { get; set; }
    }
}
