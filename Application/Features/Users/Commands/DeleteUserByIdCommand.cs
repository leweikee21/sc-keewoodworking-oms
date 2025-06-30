using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands
{
    public class DeleteUserByIdCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserByIdCommand, Response<string>>
    {
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<string>> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
        {
            return await _userService.DeleteUserAsync(request.UserId);
        }
    }

}