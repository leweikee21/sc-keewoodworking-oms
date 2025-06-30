using Application.DTOs.User;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands
{
    public class ReactivateUserCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public ReactivateUserRequest Request { get; set; }
    }

    public class ReactivateUserCommandHandler : IRequestHandler<ReactivateUserCommand, Response<string>>
    {
        private readonly IUserService _userService;

        public ReactivateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<string>> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.ReactivateUserAsync(request.UserId, request.Request);
        }
    }
}