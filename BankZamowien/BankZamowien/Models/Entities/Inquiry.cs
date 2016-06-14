using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankZamowien.Models.Entities
{
    public class Inquiry
    {
        public int Id { get; set; }
        public string RefNumber { get; set; }
        [Display(Name = "Priorytet")]
        public Priority Priority { get; set; }
        [Display(Name="Termin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = " {0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        public DateTime CreateInquiryDate { get; set; }
        public int ClientID { get; set; }
        public bool IsAnswered { get; set; }
        public virtual Client Client { get; set; }
        public virtual IEnumerable<Message> MessageList { get; set;}
    }

    public enum Priority
    {
        wysoki, normalny, niski
    }
}