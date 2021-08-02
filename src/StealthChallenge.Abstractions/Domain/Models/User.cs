namespace StealthChallenge.Abstractions.Domain.Models
{
    public class User
    {
        public User(string name, int ranking, string[] friends)
        {
            Name = name;
            Ranking = ranking;
            Friends = friends;
        }

        public string Name { get; set; }
        public string[] Friends { get; set; }
        public int Ranking { get; set; }
    }
}
