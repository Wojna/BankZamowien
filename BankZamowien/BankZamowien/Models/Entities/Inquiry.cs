using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankZamowien.Models.Entities
{
    public class Inquiry
    {
        public int Id { get; set; }
        public string RefNumber { get; set; }
        public Priority Priority { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreateInquiryDate { get; set; }
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }
        public virtual IEnumerable<Message> MessageList { get; set;}
    }

    public enum Priority
    {
        wysoki, normalny, niski
    }
}