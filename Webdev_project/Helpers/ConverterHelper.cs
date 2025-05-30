using Webdev_project.Models;

namespace Webdev_project.Helpers
{
    public class ConverterHelper
    {
        public Product_zip ConvertProductToProductZip(Product product)
        {
            // Create a new Product_zip object
            var productZip = new Product_zip
            {
                // Mapping ProductID to Product_zipId
                Product_zipId = product.ProductID,

                // Mapping Name directly
                Name = product.Name,

                // Mapping Sold to QuantitySold
                QuantitySold = product.Sold,

                // Getting the first image from the list or setting to null if the list is empty
                Image = product.ImageURL?.FirstOrDefault(),

                // Mapping Price directly
                Price = product.Price,

                // Calculating Rating (average of all ratings in the dictionary)
                Rating = product.Rating != null && product.Rating.Count > 0
                    ? (float)product.Rating.Values.Average()
                    : 0f // If no ratings, set to 0
            };

            return productZip;
        }

    }
}
