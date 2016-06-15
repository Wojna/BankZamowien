using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BankZamowien.DAL;
using BankZamowien.Models;
using BankZamowien.Models.Entities;
using BankZamowien.Models.ViewModels;
using BankZamowien.Models;

namespace BankZamowien.Controllers
{
    [Authorize]
    public class InquiriesController : Controller
    {
        private BankZamowienDbContext db = new BankZamowienDbContext();

        // GET: Inquiries
        public ActionResult Index(string searchString, bool search_All = false)
        {
            var inquiries = db.Inquiries.Include(i => i.Client);
            if(!String.IsNullOrWhiteSpace(searchString))
            {
                inquiries = inquiries.Where(i => (i.Client.Nazwisko.Contains(searchString)) || (i.Client.Imie.Contains(searchString)) || (i.Client.Email.Contains(searchString))
                                                    || (i.Client.Telefon.Contains(searchString)) || (i.RefNumber.Contains(searchString)));
            }
            if(!search_All)
            {
                inquiries = inquiries.Where(i => (i.IsAnswered == false));
            }
            List<InquiryListViewModel> viewModel = new List<InquiryListViewModel>();
            foreach(var item in inquiries)
            {
                InquiryListViewModel tmpItem = new InquiryListViewModel();
                tmpItem.Id = item.Id;
                tmpItem.RefNumber = item.RefNumber;
                tmpItem.Name = item.Client.Imie;
                tmpItem.Surname = item.Client.Nazwisko;
                tmpItem.PhoneNumber = item.Client.Telefon;
                tmpItem.Email = item.Client.Email;
                tmpItem.Priority = item.Priority;
                viewModel.Add(tmpItem);
            }
            return View(viewModel);
        }

        // GET: Inquiries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inquiry inquiry = db.Inquiries.Find(id);
            if (inquiry == null)
            {
                return HttpNotFound();
            }
            return View(inquiry);
        }

        // GET: Inquiries/Create
        public ActionResult Create(int? id)
        {
            return View();
        }

        // POST: Inquiries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Priority,ExpireDate,Name,Surname,Content")] CreateInquiryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Client client = db.Clients.SingleOrDefault(c => (c.Imie == viewModel.Name) && (c.Nazwisko == viewModel.Surname));
                if(client == null)
                {
                    client = new Client();
                    client.Imie = viewModel.Name;
                    client.Nazwisko = viewModel.Surname;
                    db.Clients.Add(client);
                    db.SaveChanges();
                    client = db.Clients.SingleOrDefault(c => (c.Imie == viewModel.Name) && (c.Nazwisko == viewModel.Surname));
                }
                //db.Inquiries.Add(inquiry);
                Inquiry inquiry = new Inquiry();
                inquiry.Priority = viewModel.Priority;
                inquiry.ExpireDate = viewModel.ExpireDate;
                inquiry.ClientID = client.Id;
                inquiry.CreateInquiryDate = DateTime.Now;

                Inquiry lastestInquiry = db.Inquiries.Where(c => (c.CreateInquiryDate.Month == DateTime.Now.Month) && (c.CreateInquiryDate.Year == DateTime.Now.Year))
                                                                    .OrderByDescending(c =>c.CreateInquiryDate)
                                                                    .FirstOrDefault();
                int numberOfInquiries = 0;
                if (lastestInquiry != null)
                {
                    string[] lastestRefNumber = lastestInquiry.RefNumber.Split('/');
                    numberOfInquiries = Convert.ToInt32(lastestRefNumber[0]);
                }
                inquiry.RefNumber = String.Format("{0}/{1}/{2}", numberOfInquiries + 1, DateTime.Now.Month, DateTime.Now.Year);
                Message msg = new Message();
                msg.Content = viewModel.Content;
                msg.PreviousMessage = null;
                msg.CreateMessageDate = DateTime.Now;
                inquiry.MessageList = new List<Message>();
                inquiry.MessageList.ToList().Add(msg);
                client.IsNonAnsweredInquiry = true;
                db.Inquiries.Add(inquiry);
                db.Messages.Add(msg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ClientID = new SelectList(db.Clients, "Id", "Imie", inquiry.ClientID);
            return View(viewModel);
        }

        // GET: Inquiries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inquiry inquiry = db.Inquiries.Find(id);
            if (inquiry == null)
            {
                return HttpNotFound();
            }
            return View(inquiry);
        }

        // POST: Inquiries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RefNumber,Priority,ExpireDate")] Inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                Inquiry newInquiry = db.Inquiries.Find(inquiry.Id);
                newInquiry.Priority = inquiry.Priority;
                newInquiry.ExpireDate = inquiry.ExpireDate;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "Id", "Imie", inquiry.ClientID);
            return View(inquiry);
        }

        // GET: Inquiries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inquiry inquiry = db.Inquiries.Find(id);
            if (inquiry == null)
            {
                return HttpNotFound();
            }
            return View(inquiry);
        }

        // POST: Inquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inquiry inquiry = db.Inquiries.Find(id);
            db.Inquiries.Remove(inquiry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
