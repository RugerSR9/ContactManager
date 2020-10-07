using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ContactManager.Data
{
    public class Email
    {
        [Key]
        public int Id { get; set; }

        public int ContactForeignKey { get; set; }

        public Contact Contact { get; set; }

        public EmailAddressType Type { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Address { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; } = false;

        public enum EmailAddressType
        {
            Personal = 1,
            Business = 2
        }
    }
}
