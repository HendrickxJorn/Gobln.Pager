using Gobln.Pager.Json.NewtonsoftTest.Model;

namespace Gobln.Pager.Json.NewtonsoftTest.Data
{
    internal class UserData
    {
        public static IEnumerable<User> TestUserList => [
                    new() { Id = 1, Name = "Tester1",   SurName= "Test", Date = new DateTime(2023, 5, 1) },
                    new() { Id = 2, Name = "Tester2",   SurName= "Test", Date = new DateTime(2023, 5, 2) },
                    new() { Id = 3, Name = "Tester3",   SurName= "Test", Date = new DateTime(2023, 5, 3) },
                    new() { Id = 4, Name = "Tester4",   SurName= "Test", Date = new DateTime(2023, 5, 4) },
                    new() { Id = 5, Name = "Tester5",   SurName= "Test", Date = new DateTime(2023, 5, 5) },
                    new() { Id = 6, Name = "Tester6",   SurName= "Test", Date = new DateTime(2023, 5, 1) },
                    new() { Id = 7, Name = "Tester7",   SurName= "Test", Date = new DateTime(2023, 5, 2) },
                    new() { Id = 8, Name = "Tester8",   SurName= "Test", Date = new DateTime(2023, 5, 3) },
                    new() { Id = 9, Name = "Tester9",   SurName= "Test", Date = new DateTime(2023, 5, 4) },
                    new() { Id = 10, Name = "Tester10", SurName= "Test", Date = new DateTime(2023, 5, 5) },
                    new() { Id = 11, Name = "Tester11", SurName= "Test", Date = new DateTime(2023, 5, 1) },
                    new() { Id = 12, Name = "Tester12", SurName= "Test", Date = new DateTime(2023, 5, 2) },
                    new() { Id = 13, Name = "Tester13", SurName= "Test", Date = new DateTime(2023, 5, 3) },
                    new() { Id = 14, Name = "Tester14", SurName= "Test", Date = new DateTime(2023, 5, 4) },
                    new() { Id = 15, Name = "Tester15", SurName= "Test", Date = new DateTime(2023, 5, 5) },
                ];
    }
}