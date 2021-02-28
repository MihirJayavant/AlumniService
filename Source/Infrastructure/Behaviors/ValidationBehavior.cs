// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Linq;
// using FluentValidation;
// using MediatR;
// using Core.Contracts.Response;

// namespace Infrastructure.Behaviors
// {
//     public class ValidationBehaior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//         where TRequest : IRequest<TResponse>
//     {
//         private readonly IEnumerable<IValidator<IRequest>> validators;

//         public ValidationBehaior(IEnumerable<IValidator<IRequest>> validators)
//                 => this.validators = validators;

//         public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//         {
//             var context = new ValidationContext(request);
//             var errors = validators.Select(x => x.Validate(context))
//                                     .SelectMany(x => x.Errors)
//                                     .Where(x => x != null);
//             if(errors.Any())
//             {
//                 throw new ValidationException(errors);
//             }
//             return next();
//         }
//     }
// }
