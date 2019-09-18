﻿using System.Collections.Generic;

namespace Cooper.Models
{
    public class Chat : Entity
    {
        public string ChatName { get; set; }
        public string PhotoURL { get; set; }
        public int UnreadMessagesAmount { get;set; }

        public IList<User> Participants { get; set; }
        public IList<Message> Messages { get; set; }
    }
}
