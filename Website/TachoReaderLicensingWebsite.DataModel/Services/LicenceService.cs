using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TachoReaderLicensingWebsite.DataModel.Services
{
    public class LicenceService
    {

        public IEnumerable<Licence> LoadLicences(Guid ClientAccessID)
        {
            using (var db = new SkillrayEntities1())
            {
                return db.Licences.Where(c => c.DateDeleted == null && c.Client.AccessID == ClientAccessID).ToList();
            }
        }

        public Guid AddLicence(Guid ClientAccessID, string ExpiryDate)
        {
            using (var db = new SkillrayEntities1())
            {
                var client = db.Clients.FirstOrDefault(c => c.AccessID == ClientAccessID);
                if (client != null)
                {
                    DateTime Expiry;
                    DateTime.TryParse(ExpiryDate, out Expiry);

                    if (Expiry > DateTime.Now)
                    {
                        var newLicence = new DataModel.Licence()
                        {
                            ClientID = client.ID,
                            DateCreated = DateTime.Now,
                            ExpiryDate = Expiry,
                            AccessID = Guid.NewGuid()
                        };

                        newLicence.LKey = this.GenerateKey(newLicence.ExpiryDate);

                        db.Licences.Add(newLicence);
                        db.SaveChanges();


                        return newLicence.AccessID;
                    }
                }



                return Guid.Empty;
            }
        }

        public void DeleteLicence(Guid AccessID)
        {
            using (var db = new SkillrayEntities1())
            {
                var existing = db.Licences.FirstOrDefault(l => l.AccessID == AccessID);
                if (existing != null)
                {
                    existing.DateDeleted = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }


        public static bool IsValid(string serial)
        {
            if (string.IsNullOrEmpty(serial))
                return false;

            if (serial.ToCharArray().Any(Char.IsLetter))
                return false;

            string padded = serial.PadRight(18, Char.Parse("0"));
            long paddedAsLong;
            if (long.TryParse(padded, out paddedAsLong))
            {
                try
                {
                    DateTime expiration = new DateTime(paddedAsLong);
                    if (expiration.Hour == 0 && expiration.Minute == 0 && expiration.Second == 0 && expiration.Millisecond == 0)
                        return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public string GenerateKey(DateTime expiration)
        {
            return expiration.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(Char.Parse("0"));
        }

        public static bool HasExpired(DateTime expirationDate)
        {
            return expirationDate.Date < DateTime.Now.Date;
        }
    }
}

