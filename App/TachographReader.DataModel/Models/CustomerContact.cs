namespace Webcal.DataModel
{
    using System;

    public class CustomerContact : BaseModel, IEquatable<CustomerContact>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string Town { get; set; }

        public string PhoneNumber { get; set; }

        public bool Equals(CustomerContact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id &&
                   string.Equals(Name, other.Name) &&
                   string.Equals(Email, other.Email) &&
                   string.Equals(Address, other.Address) &&
                   string.Equals(PostCode, other.PostCode) &&
                   string.Equals(Town, other.Town) &&
                   string.Equals(PhoneNumber, other.PhoneNumber);
        }

        public override string ToString()
        {
            return Name;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CustomerContact) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PostCode != null ? PostCode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Town != null ? Town.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}