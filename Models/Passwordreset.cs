// public class ResetPasswordRequest
// {
//     public string Phone { get; set; }
//     public string NewPassword { get; set; }
// }
// public class VerifyOtpRequest
// {
//     public string Phone { get; set; }
//     public string Otp { get; set; }
// }

using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

public class VerifyOtpRequest
{
    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(6, MinimumLength = 4)]
    public string Otp { get; set; } = string.Empty;
}
