using BankZamowien.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankZamowien.Models.ViewModels
{
    public class CreateInquiryViewModel
    {
        [Display(Name = "Priorytet")]
        public Priority Priority { get; set; }
        [Display(Name = "Termin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = " {0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        [Display(Name = "Imie")]
        public string Name { get; set; }
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Treść zapytania")]
        public string Content { get; set; }
    }
}