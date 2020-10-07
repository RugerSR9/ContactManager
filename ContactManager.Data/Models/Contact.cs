using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactManager.Data
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public List<Email> EmailAddresses { get; set; }
    }
}
