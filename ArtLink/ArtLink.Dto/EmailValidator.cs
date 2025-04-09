using System.Text.RegularExpressions;

namespace ArtLink.Dto.Validator;

public static class EmailValidator
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, EmailPattern);
    }
}
