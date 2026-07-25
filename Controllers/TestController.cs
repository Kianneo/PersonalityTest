using Microsoft.AspNetCore.Http; // Added for Session support
using Microsoft.AspNetCore.Mvc;
using PersonalityTest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PersonalityTest.Controllers
{
    public class TestController : Controller
    {
        // Helper method to check if user is logged in
        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserLoggedIn"));
        }

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
            // Protect page: Redirect to Login if session is not active
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Questions()
        {
            // Protect page: Redirect to Login if session is not active
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var questions = GetQuestions();

            var model = new TestViewModel
            {
                Questions = questions,
                // Creates a list of 16 zeros matching the 16 questions
                Answers = Enumerable.Repeat((int?)null, questions.Count).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Result()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var json = HttpContext.Session.GetString("LastResult");
            if (string.IsNullOrEmpty(json))
                return View("NoResult"); // hasn't taken the test yet

            var result = JsonSerializer.Deserialize<PersonalityResult>(json);
            return View("Result", result);
        }

        [HttpPost]
        public IActionResult Questions(TestViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            // Sum answers per category (each answer is -3..3, shift to 0..6 so bars never go negative)
            var totals = model.Questions
                .Select((q, i) => new { q.Category, Value = (model.Answers[i] ?? 0) + 3 })
                .GroupBy(x => x.Category)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));

            int e = totals.GetValueOrDefault("E"), i = totals.GetValueOrDefault("I");
            int n = totals.GetValueOrDefault("N"), s = totals.GetValueOrDefault("S");
            int t = totals.GetValueOrDefault("T"), f = totals.GetValueOrDefault("F");
            int j = totals.GetValueOrDefault("J"), p = totals.GetValueOrDefault("P");

            string type =
                (e >= i ? "E" : "I") +
                (n >= s ? "N" : "S") +
                (t >= f ? "T" : "F") +
                (j >= p ? "J" : "P");

            var (title, desc) = TypeLibrary[type];

            int Pct(int a, int b) => a + b == 0 ? 50 : (int)Math.Round(a * 100.0 / (a + b));

            var result = new PersonalityResult
            {
                Type = type,
                Title = title,
                Description = desc,
                PctE = Pct(e, i),
                PctI = 100 - Pct(e, i),
                PctN = Pct(n, s),
                PctS = 100 - Pct(n, s),
                PctT = Pct(t, f),
                PctF = 100 - Pct(t, f),
                PctJ = Pct(j, p),
                PctP = 100 - Pct(j, p)
            };

            HttpContext.Session.SetString("LastResult", JsonSerializer.Serialize(result));

            return View("Result", result);
        }

        // Add near top of class or in a separate static file
        private static readonly Dictionary<string, (string Title, string Description)> TypeLibrary = new()
        {
            ["ISTJ"] = ("Logistician", "Practical and fact-minded, reliable and dutiful in getting things done right."),
            ["ISFJ"] = ("Defender", "Warm and dedicated, always ready to protect the people they care about."),
            ["INFJ"] = ("Advocate", "Quietly idealistic, insightful, and driven by a deep sense of purpose."),
            ["INTJ"] = ("Architect", "Strategic and independent thinkers who love turning ideas into plans."),
            ["ISTP"] = ("Virtuoso", "Hands-on problem-solvers who enjoy figuring out how things work."),
            ["ISFP"] = ("Adventurer", "Gentle, artistic, and open, exploring life through experience and feeling."),
            ["INFP"] = ("Mediator", "Poetic and empathetic idealists guided by their own values."),
            ["INTP"] = ("Logician", "Curious and analytical, always chasing the logic behind everything."),
            ["ESTP"] = ("Entrepreneur", "Energetic and perceptive, thriving on action and taking risks."),
            ["ESFP"] = ("Entertainer", "Spontaneous and fun-loving, bringing energy to everyone around them."),
            ["ENFP"] = ("Campaigner", "Enthusiastic and imaginative, always finding new possibilities."),
            ["ENTP"] = ("Debater", "Sharp and inventive, loves a good intellectual challenge."),
            ["ESTJ"] = ("Executive", "Organized and decisive, natural at managing people and projects."),
            ["ESFJ"] = ("Consul", "Caring and social, focused on keeping everyone supported and connected."),
            ["ENFJ"] = ("Protagonist", "Charismatic and inspiring, motivated by helping others grow."),
            ["ENTJ"] = ("Commander", "Bold and strategic leaders, always finding a way to reach the goal.")
        };
    }
}