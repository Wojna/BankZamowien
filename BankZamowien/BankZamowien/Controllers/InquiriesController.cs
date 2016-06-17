using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BankZamowien.DAL;
using BankZamowien.Models;
using BankZamowien.Models.Entities;
using BankZamowien.Models.ViewModels;
using System.Reflection;
using Microsoft.Ajax.Utilities;

namespace BankZamowien.Controllers
{

    [Authorize]
    public class InquiriesController : Controller
    {
        private BankZamowienDbContext db = new BankZamowienDbContext();

        // GET: Inquiries
        public ActionResult Index(string searchString, bool search_All = false)
        {
            List<Inquiry> inquiries = new List<Inquiry>();
            inquiries = db.Inquiries.Include(i => i.Client).ToList();
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                Type type = typeof(Client);
                PropertyInfo[] properties = type.GetProperties();


                inquiries = inquiries.Where(i => i.Client.GetType().GetProperty(properties[1].Name).GetValue(i.Client, null).ToString().Contains(searchString) ||
                                                i.Client.GetType().GetProperty(properties[2].Name).GetValue(i.Client, null).ToString().Contains(searchString) ||
                                                (i.Client.GetType().GetProperty(properties[3].Name).GetValue(i.Client, null) != null && i.Client.GetType().GetProperty(properties[3].Name).GetValue(i.Client, null).ToString().Contains(searchString)) ||
                                                (i.Client.GetType().GetProperty(properties[4].Name).GetValue(i.Client, null) != null && i.Client.GetType().GetProperty(properties[4].Name).GetValue(i.Client, null).ToString().Contains(searchString)) ||
                                                i.RefNumber.Contains(searchString)).ToList();
                
            }
            if(!search_All)
            {
                inquiries = inquiries.Where(i => (i.IsAnswered == false)).ToList();
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

        [HttpPost]
        public ActionResult Details(string content,int ClientID, int InquiryID)
        {
            var mail = db.Clients.Where(c => c.Id == ClientID).Select(d => d.Email).FirstOrDefault();

            if(!mail.IsNullOrWhiteSpace())
               { 
                    SendMail(content,mail);
               }

            var phone = db.Clients.Where(c => c.Id == ClientID).Select(d => d.Telefon).FirstOrDefault();

            if (!phone.IsNullOrWhiteSpace())
            {
                SendSms(content,phone);
            }
            
            var PrevMsg =
                db.Messages.Where(c => c.InquiryID == InquiryID).OrderByDescending(c => c.CreateMessageDate).Select(c=> c.Id).FirstOrDefault();

            Message msg = new Message();
            msg.Content = content;
            msg.PreviousMessage = PrevMsg;
            msg.CreateMessageDate = DateTime.Now;
            msg.InquiryID = InquiryID;
            db.Messages.Add(msg);
            db.SaveChanges();


            return RedirectToAction("Details");
        }

        public void SendMail(string content, string mail)
        {
            var fromAddress = new MailAddress("do862947@gmail.com", "From Name");
            var toAddress = new MailAddress(mail, "To Name");
            const string fromPassword = "Haslo123$";
            const string subject = "Subject";
            string body = content;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("do862947@gmail.com", fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public void SendSms(string content, string phone)
        {
            try
            {
                SMSApi.Api.Client client = new SMSApi.Api.Client("damian.woinski@it-solve.pl");
                client.SetPasswordHash("b67ac19f6c5c7252aa1ff0607986aab7");

                var smsApi = new SMSApi.Api.SMSFactory(client);

                var result =
                    smsApi.ActionSend()
                        .SetText(content)
                        .SetTo(phone)
                        .SetSender("ECO") //Pole nadawcy lub typ wiadomość 'ECO', '2Way'
                        .Execute();

                //System.Console.WriteLine("Send: " + result.Count);

                string[] ids = new string[result.Count];

                for (int i = 0, l = 0; i < result.List.Count; i++)
                {
                    if (!result.List[i].isError())
                    {
                        //Nie wystąpił błąd podczas wysyłki (numer|treść|parametry... prawidłowe)
                        if (!result.List[i].isFinal())
                        {
                            //Status nie jest koncowy, może ulec zmianie
                            ids[l] = result.List[i].ID;
                            l++;
                        }
                    }
                }

                // System.Console.WriteLine("Get:");
                //result =
                //    smsApi.ActionGet()
                //        .Ids(ids)
                //        .Execute();

                //foreach (var status in result.List)
                //{
                //    System.Console.WriteLine("ID: " + status.ID + " NUmber: " + status.Number + " Points:" +
                //                             status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
                //}

                //for (int i = 0, l = 0; i < result.List.Count; i++)
                //{
                //    if (!result.List[i].isError())
                //    {
                //        var deleted =
                //            smsApi.ActionDelete()
                //                .Id(result.List[i].ID)
                //                .Execute();
                //        System.Console.WriteLine("Deleted: " + deleted.Count);
                //    }
                //}
            }
            catch (SMSApi.Api.ActionException e)
            {
                /**
                 * Błędy związane z akcją (z wyłączeniem błędów 101,102,103,105,110,1000,1001 i 8,666,999,201)
                 * http://www.smsapi.pl/sms-api/kody-bledow
                 */
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.ClientException e)
            {
                /**
                 * 101 Niepoprawne lub brak danych autoryzacji.
                 * 102 Nieprawidłowy login lub hasło
                 * 103 Brak punków dla tego użytkownika
                 * 105 Błędny adres IP
                 * 110 Usługa nie jest dostępna na danym koncie
                 * 1000 Akcja dostępna tylko dla użytkownika głównego
                 * 1001 Nieprawidłowa akcja
                 */
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.HostException e)
            {
                /* błąd po stronie servera lub problem z parsowaniem danych
                 * 
                 * 8 - Błąd w odwołaniu
                 * 666 - Wewnętrzny błąd systemu
                 * 999 - Wewnętrzny błąd systemu
                 * 201 - Wewnętrzny błąd systemu
                 * SMSApi.Api.HostException.E_JSON_DECODE - problem z parsowaniem danych
                 */
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.ProxyException e)
            {
                // błąd w komunikacji pomiedzy klientem a serverem
                System.Console.WriteLine(e.Message);
            }
        }

        // GET: Inquiries/Create
        public ActionResult Create(int? id)
        {
            CreateInquiryViewModel viewModel = new CreateInquiryViewModel();
            viewModel.Clients = new List<Client>();
            viewModel.Clients = db.Clients.ToList();
            if(id != null)
            {
                Client client = db.Clients.Find(id);
                viewModel.Name = client.Imie;
                viewModel.Surname = client.Nazwisko;
            }
            return View(viewModel);
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
