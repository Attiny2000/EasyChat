using System;
using System.ComponentModel.DataAnnotations;

namespace EasyChatServer
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
        public string MessageTime { get; set; }
    }
}
