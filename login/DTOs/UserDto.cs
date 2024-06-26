using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace login.DTOs
{
    public class UserDto
    {
        public int userID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword {  get; set; }
    }
}
