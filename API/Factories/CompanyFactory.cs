using System;
using API.Models.Companies;

namespace API.Factories
{
    public static class CompanyFactory
    {
        public static Company CreateCompany(NewUpdateCompany company)
        {
            return new Company
            {
                Id = Guid.NewGuid().ToString(),
                StoreId = company.StoreId,
                Name = company.Name,
                OrgNumber = company.OrgNumber,
                StreetAddress = company.StreetAddress,
                ZipCode = company.ZipCode,
                City = company.City,
                ContactName = company.ContactName,
                ContactEmail = company.ContactEmail,
                ContactPhone = company.ContactPhone
            };
        }

        public static Company UpdateCompany(Company company, NewUpdateCompany updateCompany)
        {
            company.Name = updateCompany.Name;
            company.OrgNumber = updateCompany.OrgNumber;
            company.StreetAddress = updateCompany.StreetAddress;
            company.ZipCode = updateCompany.ZipCode;
            company.City = updateCompany.City;
            company.ContactName = updateCompany.ContactName;
            company.ContactEmail = updateCompany.ContactEmail;
            company.ContactPhone = updateCompany.ContactPhone;

            return company;
        }
    }
}
