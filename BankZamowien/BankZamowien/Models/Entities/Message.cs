using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankZamowien.Models.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int? PreviousMessage { get; set; }
        public string Content { get; set; }
        public DateTime CreateMessageDate { get; set; }
        public int InquiryID { get; set; }
        public virtual Inquiry Inquiry { get; set;}

    }
}