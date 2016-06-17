using BankZamowien.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankZamowien.Models
{
    public class InquiryListViewModel
    {
        public int Id { get; set; }
        [Display(Name="Numer referencyjny")]
        public string RefNumber { get; set; }
        public Priority Priority { get; set; }
        [Display(Name = "Imie")]
        public string Name { get; set; }
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }
        [Display(Name = "Telefon")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:###-##-####}")]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}