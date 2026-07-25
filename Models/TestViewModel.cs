using System.Collections.Generic;

namespace PersonalityTest.Models
{
    public class TestViewModel
    {
        public List<Question> Questions { get; set; } = new();

        public List<int> Answers { get; set; } = new();
    }
}