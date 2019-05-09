using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackOverflow.Data
{
    public class StackRepository
    {
        private string _connectionString;
        public StackRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddQuestion(Question question)
        {
            using (var context = new StackContext(_connectionString))
            {
                context.Questions.Add(question);
                context.SaveChanges();
            }
        }

        public void AddAnswer(Answer answer)
        {
            using (var context = new StackContext(_connectionString))
            {
                context.Answers.Add(answer);
                context.SaveChanges();
            }
        }

        public void AddUser(User user)
        {
            using (var context = new StackContext(_connectionString))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public Tag AddTag(Tag tag)
        {
            using (var context = new StackContext(_connectionString))
            {
                context.Tags.Add(tag);
                context.SaveChanges();
                return tag;
            }
        }

        public void AddQuestionTags(IEnumerable<string> tags, int questionId)
        {
            using (var context = new StackContext(_connectionString))
            {
                foreach (string t in tags)
                {
                    Tag tag = context.Tags.FirstOrDefault(ta => ta.Content == t) ?? AddTag(new Tag { Content = t });

                    context.QuestionsTags.Add(new QuestionsTags { QuestionId = questionId, TagId = tag.Id });
                    context.SaveChanges();
                }
            }
        }

        public int AddLikedQuestion(int questionId, int userId)
        {
            using (var context = new StackContext(_connectionString))
            {
                if (context.LikedQuestions.Any(l => l.QuestionId == questionId && l.UserId == userId))
                {
                    return context.LikedQuestions.Where(l => l.QuestionId == questionId).ToList().Count();
                }
                context.LikedQuestions.Add(new LikedQuestions { QuestionId = questionId, UserId = userId });
                context.SaveChanges();
                return context.LikedQuestions.Where(l => l.QuestionId == questionId).ToList().Count();
            }
        }

        public int AddLikedAnswers(int answerId, int userId)
        {
            using (var context = new StackContext(_connectionString))
            {
                if (context.LikedAnswers.Any(l => l.UserId == userId && l.AnswerId == answerId))
                {
                    return context.LikedAnswers.Where(l => l.AnswerId == answerId).ToList().Count();
                }
                context.LikedAnswers.Add(new LikedAnswers { AnswerId = answerId, UserId = userId });
                context.SaveChanges();
                return context.LikedAnswers.Where(l => l.AnswerId == answerId).ToList().Count();
            }
        }

        public User VerifyLogIn(string email, string password)
        {
            using (var context = new StackContext(_connectionString))
            {
                User user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return null;
                }
                bool GoodPassword = BCrypt.Net.BCrypt.Verify(password, user.PassWord
);
                if (!GoodPassword)
                {
                    return null;
                }
                return user;
            }
        }

        public User GetUserByEmail(string email)
        {
            using (var context = new StackContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public IEnumerable<Question> GetQuestions()
        {
            using (var context = new StackContext(_connectionString))
            {
                return context.Questions
                    .Include(a => a.Answers)
                    .Include(u => u.User)
                    .Include(q => q.QuestionsTags)
                    .ThenInclude(q => q.Tag)
                    .Include(l => l.LikedQuestions)
                    .OrderByDescending(q => q.Date)
                    .ToList();
            }
        }

        public Question GetQuestionById(int questionid)
        {
            using (var context = new StackContext(_connectionString))
            {
                return context.Questions
                    .Include(q => q.Answers)
                    .ThenInclude(l => l.LikedAnswers)
                    .Include(a => a.Answers)
                    .ThenInclude(u => u.User)
                    .Include(q => q.QuestionsTags)
                    .ThenInclude(t => t.Tag)
                    .Include(l => l.LikedQuestions)
                    .Include(u => u.User)
                    .FirstOrDefault(q => q.Id == questionid);
            }
        }

    }
}
