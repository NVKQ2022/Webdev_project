using System.ComponentModel.DataAnnotations;

namespace Webdev_project.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Email{ get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Salt {  get; set; }

        public bool IsAdmin { get; set; }
        public User()
        {

        }

    }
}
