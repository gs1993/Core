﻿using CSharpFunctionalExtensions;
using System;

namespace WebApi.Entities.Site
{
    public class Site : BaseEntity
    {
        public string Name { get; private set; }
        public virtual Address Address { get; private set; }

        protected Site() { }
        private Site(string name, Address address, DateTime createDate) : base(createDate)
        {
            Name = name;
            Address = address;
        }

        public static Result<Site> Create(string name, Address address, DateTime createDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Site>("Invalid site name");
            if (address is null)
                return Result.Failure<Site>("Invalid site address");

            return Result.Success(new Site(name, address, createDate));
        }
    }
}
