using System;
using Core.Contracts.Response;
using MediatR;

namespace Infrastructure.Commands
{
    public class AddExamCommand : IRequest<Response<ExamResponse>>
    {
        public string ExamName { get; set; }
        public int Score { get; set; }
        public int Year { get; set; }
        public int StudentId { get; set; }
    }
}
