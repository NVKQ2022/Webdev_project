namespace Webdev_project.Models
{
    public class Review
    {
        ReviewId Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<string>? MediaURLs { get; set; }

    }
    struct ReviewId
    { 
        public int ProductID { get; set; }
        public int UserID { get; set; }
    }
}
