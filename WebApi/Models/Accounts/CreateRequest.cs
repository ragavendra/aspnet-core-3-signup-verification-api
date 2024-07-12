using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Models.Accounts
{
    public class CreateRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Safer to use char[] for conf info as it is mutable
        /* Getting like this
        {
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-c629d8c23bb3aaeb1e82011e82b8d4f8-3ba17aea2d610234-00",
  "errors": {
    "$.password": [
      "The JSON value could not be converted to System.Char[]. Path: $.password | LineNumber: 6 | BytePositionInLine: 22."
    ]
  }
}
        */
        [Required]
        [MinLength(6)]
        public char[] Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
