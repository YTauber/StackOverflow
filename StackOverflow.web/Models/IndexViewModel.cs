using StackOverflow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackOverflow.Web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Question> Questions { get; set; }
    }

    public class ViewQuestionModel
    {
        public Question Question { get; set; }
        public User User { get; set; }
    }

    public class LogInViewModel
    {
        public string ReturnUrl { get; set; }
    }
}
