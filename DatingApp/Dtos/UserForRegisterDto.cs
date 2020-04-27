using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos
{
    public class UserForRegisterDto
    {

        [Required]
        public string  UserName { get; set; }

       [Required]
       [StringLength(8,MinimumLength=4,ErrorMessage="You must passord...")]
        public string  Password { get; set; }
    }
}