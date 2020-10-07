using ContactManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ContactManager.BusinessLogic
{
    public class ContactsService : IContactsService
    {
        private readonly ContactManagerDbContext _context;

        public ContactsService(ContactManagerDbContext context)
        {
            _context = context;
        }

        public Contact CreateContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public bool DeleteContact(int id)
        {
            if (_context.Contacts.Any(o => o.Id == id))
            {
                _context.Contacts.Remove(_context.Contacts.First(o => o.Id == id));
                _context.SaveChanges();
                return true;
            }
            else
                return false;
        }

        public bool Exists(int id)
        {
            return _context.Contacts.Any(o => o.Id == id);
        }

        public IEnumerable<Contact> GetContacts()
        {
            return _context.Contacts.Include(o => o.EmailAddresses).ToList();
        }

        public Contact GetContact(int id)
        {
            return _context.Contacts.Include(o => o.EmailAddresses).FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Contact> SearchContacts(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Replace(" ", ""); // replace spaces and make lowercase since this search isn't very complex
            return _context.Contacts.Include(o => o.EmailAddresses)
                .Where(o => (o.FirstName + o.LastName).Replace(" ", "").ToLower().Contains(searchTerm) ||
                o.EmailAddresses.Any(o => o.Address.ToLower().Contains(searchTerm)))
                .ToList();
        }

        public Contact SaveContact(Contact contact)
        {
            if (Exists(contact.Id))
            {
                // remove deleted existing entries, ignore deleted temp entries
                if (contact.EmailAddresses.Any(o => o.IsDeleted && o.Id != 0))
                    foreach (Email email in contact.EmailAddresses.Where(o => o.IsDeleted && o.Id != 0))
                        _context.EmailAddresses.RemoveRange(email);

                _context.Contacts.Update(contact);
                _context.SaveChanges();
                return contact;
            }
            else
            {
                contact = CreateContact(contact);
                return contact;
            }
        }

        public void UpdateDisplayName(Contact contact)
        {
            if (Exists(contact.Id))
            {
                contact.DisplayName = contact.LastName + ", " + contact.FirstName;
                _context.Contacts.Update(contact);
                _context.SaveChanges();
            }    
        }
    }
}
