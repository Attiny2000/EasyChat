namespace EasyChatServer
{
    using System;
    using System.Data.Entity;

    public class DataContext : DbContext
    {
        public DataContext(): base("name=DataContext"){}

        public DbSet<User> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}