using Application.DTOs.User;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries
{
    public class GetAllUsersNameQuery : IRequest<Response<List<string>>>
    {
    }

    public class GetAllUsersNameQueryHandler : IRequestHandler<GetAllUsersNameQuery, Response<List<string>>>
    {
        private readonly IUserService _userService;

        public GetAllUsersNameQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<List<string>>> Handle(GetAllUsersNameQuery request, CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllUserNamesAsync();

            return new Response<List<string>>(result);
        }
    }

}