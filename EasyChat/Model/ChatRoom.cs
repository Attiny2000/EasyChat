using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyChat
{
    public class ChatRoom
    {
        [Key]
        public int ChatRoomId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        public ICollection<User> MembersArray { get; set; }
        public ChatRoom()
        {
            MembersArray = new List<User>();
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
