﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hipda.BackgroundTask.Models
{
    public class AccountItemModel
    {
        public AccountItemModel(string username, string password, int questionId, string answer, bool isDefault)
        {
            this.Username = username;
            this.Password = password;
            this.QuestionId = questionId;
            this.Answer = answer;
            this.IsDefault = isDefault;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public int QuestionId { get; set; }

        public string Answer { get; set; }

        public bool IsDefault { get; set; }
    }
}
