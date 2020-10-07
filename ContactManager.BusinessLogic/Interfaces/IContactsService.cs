using ContactManager.Data;
using System.Collections.Generic;

namespace ContactManager.BusinessLogic
{
    public interface IContactsService
    {
        Contact CreateContact(Contact contact);
        IEnumerable<Contact> GetContacts();
        Contact GetContact(int id);
        bool Exists(int id);
        IEnumerable<Contact> SearchContacts(string searchString);
        Contact SaveContact(Contact contact);
        void UpdateDisplayName(Contact contact);
        bool DeleteContact(int id);
    }
}
