namespace EasyChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatRooms",
                c => new
                    {
                        ChatRoomId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 24),
                    })
                .PrimaryKey(t => t.ChatRoomId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 24),
                        Password = c.String(nullable: false, maxLength: 24),
                        Photo = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Login, unique: true);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        MessageText = c.String(nullable: false),
                        MessageTime = c.String(nullable: false),
                        ChatRoom_ChatRoomId = c.Int(nullable: false),
                        Sender_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.ChatRooms", t => t.ChatRoom_ChatRoomId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.Sender_UserId, cascadeDelete: true)
                .Index(t => t.ChatRoom_ChatRoomId)
                .Index(t => t.Sender_UserId);
            
            CreateTable(
                "dbo.UserChatRooms",
                c => new
                    {
                        User_UserId = c.Int(nullable: false),
                        ChatRoom_ChatRoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_UserId, t.ChatRoom_ChatRoomId })
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .ForeignKey("dbo.ChatRooms", t => t.ChatRoom_ChatRoomId, cascadeDelete: true)
                .Index(t => t.User_UserId)
                .Index(t => t.ChatRoom_ChatRoomId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Sender_UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "ChatRoom_ChatRoomId", "dbo.ChatRooms");
            DropForeignKey("dbo.UserChatRooms", "ChatRoom_ChatRoomId", "dbo.ChatRooms");
            DropForeignKey("dbo.UserChatRooms", "User_UserId", "dbo.Users");
            DropIndex("dbo.UserChatRooms", new[] { "ChatRoom_ChatRoomId" });
            DropIndex("dbo.UserChatRooms", new[] { "User_UserId" });
            DropIndex("dbo.Messages", new[] { "Sender_UserId" });
            DropIndex("dbo.Messages", new[] { "ChatRoom_ChatRoomId" });
            DropIndex("dbo.Users", new[] { "Login" });
            DropIndex("dbo.ChatRooms", new[] { "Name" });
            DropTable("dbo.UserChatRooms");
            DropTable("dbo.Messages");
            DropTable("dbo.Users");
            DropTable("dbo.ChatRooms");
        }
    }
}
