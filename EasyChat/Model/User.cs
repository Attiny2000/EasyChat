using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyChat
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Nick { get; set; }
        public ICollection<ChatRoom> ChatRooms { get; set; }
        public User()
        {
            ChatRooms = new List<ChatRoom>();
        }
        public static explicit operator User(List<User> v)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return this.Nick;
        }
    }
}
