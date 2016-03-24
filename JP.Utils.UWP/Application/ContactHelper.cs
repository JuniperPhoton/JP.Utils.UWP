using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Popups;

namespace JP.Utils.Application
{
    public static class ContactHelper
    {
        public static async Task<IReadOnlyList<Contact>> GetAllContactsAsync()
        {
            var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

            if (contactStore == null)
            {
                MessageDialog connectionWarning =
                    new MessageDialog("应用需要访问你的所有联系人，请允许");
                await connectionWarning.ShowAsync();

                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-contacts"));
                return null;
            }

            var contactReader = contactStore.GetContactReader();

            var contactBatch = await contactReader.ReadBatchAsync();

            var list = new List<Contact>();

            while (contactBatch.Contacts.Count != 0)
            {
                foreach (var contact in contactBatch.Contacts)
                {
                    list.Add(contact);
                }
                contactBatch = await contactReader.ReadBatchAsync();
            }

            var c = list.Count;

            return list;
        }
    }
}
