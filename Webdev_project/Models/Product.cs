namespace Webdev_project.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Color { get; set; }
        public int Price { get; set; }
        public List<string> ImageURL { get; set; }
        public Dictionary<string, string> Detail { get; set; }
    }

}
