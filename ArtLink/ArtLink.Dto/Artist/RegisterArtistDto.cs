using System;
using System.Text.Json.Serialization;
using ArtLink.Dto.Validator;

namespace ArtLink.Dto.Artist;

public class RegisterArtistDto
{
    public RegisterArtistDto(string firstName, 
        string lastName, 
        string email, 
        string bio, 
        string profilePicturePath, 
        int experience)
    {
        if (!EmailValidator.IsValidEmail(email))
        {
            throw new ArgumentException("Email is not valid.", nameof(email));
        }

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Bio = bio;
        ProfilePicturePath = profilePicturePath;
        Experience = experience;
    }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("bio")]
    public string Bio { get; set; }

    [JsonPropertyName("profile_picture_path")]
    public string ProfilePicturePath { get; set; }

    [JsonPropertyName("experience")]
    public int Experience { get; set; }
}
