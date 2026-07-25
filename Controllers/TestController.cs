using Microsoft.AspNetCore.Mvc;
using PersonalityTest.Models;
using System.Linq;

namespace PersonalityTest.Controllers
{
    public class TestController : Controller
    {
        private List<Question> GetQuestions()
        {
            return new List<Question>
            {
                new Question { Id = 1, Text = "I enjoy meeting new people.", Category = "E" },
                new Question { Id = 2, Text = "I prefer quiet places.", Category = "I" },
                new Question { Id = 3, Text = "I make decisions using logic.", Category = "T" },
                new Question { Id = 4, Text = "I rely on emotions.", Category = "F" }
            };
        }

        public IActionResult Instructions()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Questions()
        {
            var questions = GetQuestions();

            var model = new TestViewModel
            {
                Questions = questions,
                Answers = Enumerable.Repeat((int?)null, questions.Count).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Questions(TestViewModel model)
        {
            int total = model.Answers.Sum(a => a ?? 0);

            PersonalityResult result;

            if (total >= 0)
            {
                result = new PersonalityResult
                {
                    Type = "ENTJ",
                    Title = "Commander",
                    Description = "Confident and strategic leader."
                };
            }
            else
            {
                result = new PersonalityResult
                {
                    Type = "INFP",
                    Title = "Mediator",
                    Description = "Creative and thoughtful."
                };
            }

            return View("Result", result);
        }
    }
}