﻿using System;
using System.ComponentModel.DataAnnotations;

namespace EasyChat
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [Required]
        public string MessageText { get; set; }
        [Required]
        public User Sender { get; set; }
        [Required]
        public ChatRoom ChatRoom { get; set; }
        [Required]
        public DateTime MessageTime { get; set; }

    }
}
