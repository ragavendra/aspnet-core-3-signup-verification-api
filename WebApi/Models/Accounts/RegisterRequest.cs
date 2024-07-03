using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebApi.Models.Accounts
{
    public class RegisterRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        // [PasswordRegex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$", ErrorMessage = "{0} value does not match the regex {1}.")]
        [PasswordRegex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$", ErrorMessage = "{0} value does not match the requirements. Min 8 chars in length, one lower, one upper and one special char")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
    }

    public class PasswordRegexAttribute : ValidationAttribute
    {
        readonly string _regex;

        public string Regex_
        {
            get { return _regex; }
        }

        public PasswordRegexAttribute(string regex)
        {
            _regex = regex;
        }

        // Private fields.
        public override bool IsValid(object value)
        {
            var password = (String)value;
            bool result = false;

            // StringValidator regexStringValidator = new StringValidator(1, 6, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$");
            // Regex regex = new Regex(@"^(.{0,7}|[^0-9]*|[^A-Z])$");
            Regex regex = new Regex(this.Regex_);
            Match match = regex.Match(password);

            if (match.Success)
                result = true;

            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              ErrorMessageString, name, this.Regex_);
        }
    }
}
