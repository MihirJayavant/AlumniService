using System;
using Core.Contracts.Response;
using MediatR;

namespace Infrastructure.Commands
{
    public class AddCompanyCommand : IRequest<Response<CompanyResponse>>
    {
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public int YearOfJoining { get; set; }
        public long AnnualSalary { get; set; }
        public int StudentId { get; set; }
    }
}
