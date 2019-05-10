using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyChatServer
{
    public class ChatRoom
    {
        [Key]
        public int ChatRoomId { get; set; }
        [Index(IsUnique = true)]
        [StringLength(24)]
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
