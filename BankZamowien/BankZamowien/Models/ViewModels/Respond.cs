using BankZamowien.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankZamowien.Models.ViewModels
{
    public class Respond
    {
        //public Inquiry Inquiry { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public int ClientID { get; set; }
        public int InquiryID { get; set; }
            
    }
}