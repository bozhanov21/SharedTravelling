using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class CustomEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Полето е задължително");
        }

        string? email = value as string;
        if (email == null)
        {
            return new ValidationResult("Невалиден имейл");
        }

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic email regex

        if (!Regex.IsMatch(email, emailPattern))
        {
            return new ValidationResult("Невалиден имейл");
        }

        return ValidationResult.Success;
    }
}
