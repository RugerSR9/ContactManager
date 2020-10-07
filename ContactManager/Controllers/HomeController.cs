using System;
using Microsoft.AspNetCore.Mvc;
using ContactManager.Data;
using ContactManager.BusinessLogic;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace ContactManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactsService _contacts;

        public HomeController(IContactsService contacts)
        {
            _contacts = contacts;
        }

        public IActionResult Index()
        {
            ViewData["Contacts"] = _contacts.GetContacts();
            return View();
        }

        [HttpPost]
        public ActionResult GetContacts(string searchTerm = null)
        {
            if (String.IsNullOrEmpty(searchTerm))
                ViewData["Contacts"] = _contacts.GetContacts();
            else
                ViewData["Contacts"] = _contacts.SearchContacts(searchTerm);

            return new PartialViewResult
            {
                ViewName = "_ContactsList",
                ViewData = ViewData,
            };
        }

        [HttpPost]
        public ActionResult EditContact(int id)
        {
            try
            {
                if (id == 0)
                    return PartialView("_EditCreateContact", new Contact());
                else
                    return PartialView("_EditCreateContact", _contacts.GetContact(id));
            }
            catch
            {
                return Json("An error occurred while fetching the contact.");
            }
        }

        [HttpPost]
        public ActionResult EditCreateContact(Contact contact)
        {
            try
            {
                if (TryValidateModel(contact))
                {
                    _contacts.SaveContact(contact);

                    // do hangfire background job
                    BackgroundJob.Enqueue(() => _contacts.UpdateDisplayName(contact));

                    return Json(true);
                }
                else
                    return Json("An error occurred: Check your inputs and try again.");
            }
            catch (Exception ex)
            {
                return Json("An error occurred while creating the contact, try again.");
            }
        }

        [HttpPost]
        public ActionResult ImportFile(IFormFile file)
        {
            try
            {
                if (file == null)
                    return Json("No file detected.");

                List<string> csvContents = new List<string>();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        csvContents.Add(reader.ReadLine());
                }

                csvContents.RemoveAll(i => String.IsNullOrEmpty(i)); //  remove empty rows
                Regex csvSplit = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                List<string> headers = csvSplit.Split(csvContents[0]).Select(o => o.Trim('"')).ToList();
                csvContents.RemoveAt(0); // remove header row

                List<string> rowDataSplit;
                List<Contact> importData = new List<Contact>();

                while (csvContents.Count > 0)
                {
                    rowDataSplit = csvSplit.Split(csvContents[0]).ToList();

                    // todo: make import support multiple email addresses
                    Contact newImportContact = new Contact()
                    {
                        FirstName = rowDataSplit[headers.IndexOf("FirstName")].ToString().Trim('"'),
                        LastName = rowDataSplit[headers.IndexOf("LastName")].ToString().Trim('"'),
                        EmailAddresses = new List<Email> {
                            new Email(){
                                Address = rowDataSplit[headers.IndexOf("Email")].ToString().Trim('"'),
                                Type = (Email.EmailAddressType)Enum.Parse(typeof(Email.EmailAddressType), rowDataSplit[headers.IndexOf("EmailType")].ToString().Trim('"'))
                            }
                        }
                    };

                    importData.Add(newImportContact);

                    csvContents.RemoveAt(0);
                }

                foreach (Contact contact in importData)
                {
                    _contacts.CreateContact(contact);
                    _contacts.UpdateDisplayName(contact);
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json("Something went wrong. Check your data and try again.");
            }
        }
    }
}