using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyChatServer
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(24)]
        public string Login { get; set; }
        [Required]
        [StringLength(24)]
        public string Password { get; set; }
        public string Photo { get; set; }
        public ICollection<ChatRoom> ChatRooms { get; set; }
        public User()
        {
            ChatRooms = new List<ChatRoom>();
        }
        public User(string login, string password, string photo)
        {
            Login = login;
            Password = password;
            Photo = photo;
            ChatRooms = new List<ChatRoom>();
        }
        public static explicit operator User(List<User> v)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return this.Login;
        }
    }
}
