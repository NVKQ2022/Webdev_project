namespace Webdev_project.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<string>? MediaURLs {  get; set; }
    }
}
