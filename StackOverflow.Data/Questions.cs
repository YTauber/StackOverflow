using System;
using System.Collections.Generic;

namespace StackOverflow.Data
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<Answer> Answers { get; set; }
        public List<QuestionsTags> QuestionsTags { get; set; }
        public List<LikedQuestions> LikedQuestions { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<LikedAnswers> LikedAnswers { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public List<QuestionsTags> QuestionsTags { get; set; }
    }

    public class QuestionsTags
    {
        public int QuestionId { get; set; }
        public int TagId { get; set; }
        public Question Question { get; set; }
        public Tag Tag { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }

        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }
        public List<LikedQuestions> LikedQuestions { get; set; }
        public List<LikedAnswers> LikedAnswers { get; set; }
    }

    public class LikedQuestions
    {
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public User User { get; set; }
        public Question Question { get; set; }
    }

    public class LikedAnswers
    {
        public int UserId { get; set; }
        public int AnswerId { get; set; }
        public User User { get; set; }
        public Answer Answer { get; set; }
    }
}
