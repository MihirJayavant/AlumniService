using System;
using Core.Contracts.Response;
using MediatR;

namespace Infrastructure.Commands
{
    public class AddFurtherStudyCommand : IRequest<Response<FurtherStudyResponse>>
    {
        public string InstituteName { get; set; }
        public string Degree { get; set; }
        public int AdmissionYear { get; set; }
        public int PassingYear { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int StudentId { get; set; }
    }
}
