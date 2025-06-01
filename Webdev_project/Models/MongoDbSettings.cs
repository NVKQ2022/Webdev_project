namespace Webdev_project.Models
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ProductCollectionName { get; set; }

        public string OrderCollectionName { get; set; }
        public string ReviewCollectionName { set; get; }

        public string CategoryCollectionName { get; set; }
    }
}
