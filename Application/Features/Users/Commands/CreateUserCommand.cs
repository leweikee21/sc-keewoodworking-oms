using Application.DTOs.Account;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<Response<string>>
    {
        public RegisterRequest Request { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Response<string>>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(request.Request);
        }
    }
}