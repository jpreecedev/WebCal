using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TachoReaderLicensingWebsite.DataModel.Services
{
    public class ClientService
    {
        public IEnumerable<Client> LoadClients(string SearchFilter)
        {

            using (var db = new SkillrayEntities1())
            {
                if (string.IsNullOrEmpty(SearchFilter))
                {
                    return db.Clients.Where(c => c.DateDeleted == null).ToList();
                }
                else
                {
                    return db.Clients.Where(c => c.DateDeleted == null && c.Name.StartsWith(SearchFilter)).ToList();
                }
            }

        }

        public Client LoadDetails(Guid ClientAccessID)
        {

            using (var db = new SkillrayEntities1())
            {
                return db.Clients.FirstOrDefault(c => c.DateDeleted == null && c.AccessID == ClientAccessID);
            }

        }


        public Guid AddClient(string Name)
        {

            using (var db = new SkillrayEntities1())
            {

                var newClient = new DataModel.Client()
                {
                    DateCreated = DateTime.Now,
                    AccessID = Guid.NewGuid(),
                    Name = (Name.Length >= 50 ? Name.Substring(0, 50) : Name)
                };
                
                db.Clients.Add(newClient);
                db.SaveChanges();
                
                return newClient.AccessID;
            }
        }
        

        public Guid UpdateClientName(Guid ID, string Name)
        {
            var result = Guid.Empty;

            using (var db = new SkillrayEntities1())
            {
                var existing = db.Clients.FirstOrDefault(c => c.AccessID == ID);

                if (existing != null)
                {
                    existing.Name = (Name.Length >= 50 ? Name.Substring(0, 50) : Name);

                    db.SaveChanges();

                    result = existing.AccessID;
                }
            }

            return result;
        }

        public void DeleteClient(Guid ID)
        {
            using (var db = new SkillrayEntities1())
            {
                var existing = db.Clients.FirstOrDefault(c => c.AccessID == ID);

                if (existing != null)
                {
                    existing.DateDeleted = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }
    }
}
