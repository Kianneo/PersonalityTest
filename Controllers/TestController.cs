using Microsoft.AspNetCore.Mvc;
using PersonalityTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace PersonalityTest.Controllers
{
    public class TestController : Controller
    {
        // 1. Updated with all 16 questions organized by category
        private List<Question> GetQuestions()
        {
            return new List<Question>
            {
                // Extraversion (E) vs Introversion (I)
                new Question { Id = 1, Category = "E", Text = "You regularly make new friends and feel energized in large groups." },
                new Question { Id = 2, Category = "I", Text = "You prefer quiet, low-key activities over lively group events." },
                new Question { Id = 3, Category = "E", Text = "You usually initiate conversations when meeting someone new." },
                new Question { Id = 4, Category = "I", Text = "You feel drained if you spend too much time interacting with crowds. " },

                // Intuition (N) vs Sensing (S)
                new Question { Id = 5, Category = "N", Text = "You frequently spend time exploring abstract or theoretical ideas." },
                new Question { Id = 6, Category = "S", Text = "You focus more on real-world facts and concrete details." },
                new Question { Id = 7, Category = "N", Text = "You prefer looking at the big picture rather than step-by-step details." },
                new Question { Id = 8, Category = "S", Text = "You rely on past experiences and proven methods to solve problems." },

                // Thinking (T) vs Feeling (F)
                new Question { Id = 9, Category = "T", Text = "You prioritize objective logic and efficiency over emotional impact." },
                new Question { Id = 10, Category = "F", Text = "You prioritize empathy, harmony, and how choices affect people's feelings." },
                new Question { Id = 11, Category = "T", Text = "In discussions, truth and accuracy are more important than keeping people pleased." },
                new Question { Id = 12, Category = "F", Text = "You often rely on your gut instinct and personal values when making choices." },

                // Judging (J) vs Prospecting (P)
                new Question { Id = 13, Category = "J", Text = "You like to keep a detailed schedule and prefer concrete plans." },
                new Question { Id = 14, Category = "P", Text = "You prefer keeping your options open and acting spontaneously." },
                new Question { Id = 15, Category = "J", Text = "You complete tasks well ahead of deadlines rather than at the last minute." },
                new Question { Id = 16, Category = "P", Text = "You adapt quickly to unexpected changes and enjoy flexible workflows." }
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
                // Creates a list of 16 zeros matching the 16 questions
                Answers = Enumerable.Repeat((int?)null, questions.Count).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Questions(TestViewModel model)
        {
            // Simple scoring logic based on total score
            int total = model.Answers != null ? model.Answers.Sum(a => a ?? 0) : 0;

            PersonalityResult result;

            // Rating scale: 1 (Disagree) to 5 (Agree) per question across 16 questions
            // Max score = 80, Min score = 16
            if (total >= 48)
            {
                result = new PersonalityResult
                {
                    Type = "ENTJ",
                    Title = "Commander",
                    Description = "Bold, imaginative, and strong-willed leaders, always finding a way – or making one."
                };
            }
            else
            {
                result = new PersonalityResult
                {
                    Type = "INFP",
                    Title = "Mediator",
                    Description = "Poetic, kind, and altruistic people, always eager to help a good cause."
                };
            }

            return View("Result", result);
        }
    }
}