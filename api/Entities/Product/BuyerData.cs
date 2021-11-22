using CSharpFunctionalExtensions;
using System.Collections.Generic;
using WebApi.Entities.Shared;

namespace WebApi.Entities.Product
{
    public class BuyerData : ValueObject
    {
        public Email Email { get; }
        public virtual Name Name { get; }
        public virtual PhoneNumber PhoneNumber { get; set; }

        protected BuyerData() { }
        private BuyerData(Email email, Name name, PhoneNumber phoneNumber)
        {
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
        }

        public static Result<BuyerData> Create(Email email, Name name, PhoneNumber phoneNumber)
        {
            if (email == null)
                return Result.Failure<BuyerData>("Email cannot be empty");
            if (name == null)
                return Result.Failure<BuyerData>("Name cannot be empty");
            if (phoneNumber == null)
                return Result.Failure<BuyerData>("Email cannot be empty");

            return Result.Success(new BuyerData(email, name, phoneNumber));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Email;
            yield return Name;
            yield return PhoneNumber;
        }
    }
}
