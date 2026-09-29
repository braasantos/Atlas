namespace Atlas.Models
{
    public enum Status
    {
        Reading,
        Read,
        Abandoned,
        ToRead
    }
    public class Books
    {
        public string? Name {get; set;}
        public int Id {get; set;}
        public string? ISBN {get; set;}
        public Status BookStatus {get; set;}

        // EF Core uses this constructor when materializing a book from SQLite.
        public Books() { }

        public Books(int id, string name, string isbn, Status status)
        {
            Id = id;
            Name = name;
            ISBN = isbn;
            BookStatus = status;
        }
    }
}
