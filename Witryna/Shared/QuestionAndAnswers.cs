using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared
{
    public class QuestionAndAnswers
    {
        public question Question { get; set; }
        public List<answer> Answers { get; set; }
    }
}
