using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankZamowien.Models.Entities
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string Imie { get; set; }
        [Required]
        public string Nazwisko { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public virtual IEnumerable<Inquiry> InquiryList { get; set; }
    }
}