using CSharpFunctionalExtensions;
using System;

namespace WebApi.Entities
{
    public abstract class BaseEntity : Entity
    {
        public DateTime CreateDate { get; private set; }
        public DateTime? LastUpdateDate { get; protected set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeleteDate { get; private set; }

        protected BaseEntity() { }
        protected BaseEntity(DateTime createdDate)
        {
            CreateDate = createdDate;
        }

        public void Delete(DateTime deleteDate)
        {
            if (IsDeleted) 
                return;
            IsDeleted = true;
            DeleteDate = deleteDate;
        }
    }
}