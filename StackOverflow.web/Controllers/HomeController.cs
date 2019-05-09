using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackOverflow.Data;
using StackOverflow.web.Models;
using StackOverflow.Web.Models;

namespace StackOverflow.web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index()
        {
            StackRepository repo = new StackRepository(_connectionString);
            IndexViewModel vm = new IndexViewModel();

            vm.Questions = repo.GetQuestions();

            return View(vm);
        }

        public IActionResult ViewQuestion(int questionId)
        {
            StackRepository repo = new StackRepository(_connectionString);
            ViewQuestionModel vm = new ViewQuestionModel();
            vm.Question = repo.GetQuestionById(questionId);
            vm.User = repo.GetUserByEmail(User.Identity.Name);
            return View(vm);
        }

        public IActionResult LogIn(string returnUrl)
        {
            return View(new LogInViewModel { ReturnUrl = returnUrl});
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [Authorize]
        public IActionResult AskQuestion()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult LikeAnswer(int answerId)
        {
            StackRepository repo = new StackRepository(_connectionString);
            User user = repo.GetUserByEmail(User.Identity.Name);
            int likes = repo.AddLikedAnswers(answerId, user.Id);
            return Json(new { Likes = likes });
        }

        [HttpPost]
        public IActionResult LikeQuestion(int questionId)
        {
            StackRepository repo = new StackRepository(_connectionString);
            User user = repo.GetUserByEmail(User.Identity.Name);
            int likes = repo.AddLikedQuestion(questionId, user.Id);
            return Json(new { Likes = likes });

        }

        [HttpPost]
        public IActionResult AddAnswer(Answer answer)
        {
            StackRepository repo = new StackRepository(_connectionString);
            answer.UserId = repo.GetUserByEmail(User.Identity.Name).Id;
            answer.Date = DateTime.Now;
            repo.AddAnswer(answer);
            return Redirect($"/home/viewquestion?questionid={answer.QuestionId}");
        }

        [HttpPost]
        public IActionResult AddQuestion(Question question, IEnumerable<string> tags)
        {
            StackRepository repo = new StackRepository(_connectionString);

            question.Date = DateTime.Now;
            question.UserId = repo.GetUserByEmail(User.Identity.Name).Id;
            repo.AddQuestion(question);
            repo.AddQuestionTags(tags, question.Id);
            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult AddSignUp(User user, string password)
        {
            user.PassWord = BCrypt.Net.BCrypt.HashPassword(password);
            StackRepository repo = new StackRepository(_connectionString);
            repo.AddUser(user);
            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult AddLogIn(string email, string password, string returnUrl)
        {
            StackRepository repo = new StackRepository(_connectionString);
            User user = repo.VerifyLogIn(email, password);
            if (user == null)
            {
                return Redirect("/home/login");
            }

            var claims = new List<Claim>
                {
                    new Claim("user", email)
                };
            HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("/home/index");
        }
    }
}
