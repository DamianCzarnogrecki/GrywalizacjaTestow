using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Shared
{
    public class EnteredUserData
    {
        [Required(ErrorMessage = "WPROWADŹ LOGIN")]
        [MinLength(3, ErrorMessage = "LOGIN JEST ZBYT KRÓTKI")]
        [MaxLength(32, ErrorMessage = "LOGIN JEST ZBYT DŁUGI")]
        [RegularExpression(@"^[a-zA-Z0-9]", ErrorMessage = "LOGIN ZAWIERA NIEDOZWOLONE ZNAKI")]
        public string Login { get; set; } = "";
        [Required(ErrorMessage = "WPROWADŹ HASŁO")]
        [MinLength(5, ErrorMessage = "HASŁO JEST ZBYT KRÓTKIE")]
        [MaxLength(32, ErrorMessage = "HASŁO JEST ZBYT DŁUGIE")]
        [RegularExpression(@"^[a-zA-Z0-9]", ErrorMessage = "HASŁO ZAWIERA NIEDOZWOLONE ZNAKI")]
        public string Password { get; set; } = "";
    }
}