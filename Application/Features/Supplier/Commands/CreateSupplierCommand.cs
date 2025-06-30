using Application.DTOs.Account;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Supplier.Commands
{
    public partial class CreateSupplierCommand : IRequest<Response<int>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
    }

    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Response<int>>
    {
        private readonly ISupplierRepositoryAsync _supplierRepository;
        private readonly IMapper _mapper;

        public CreateSupplierCommandHandler(ISupplierRepositoryAsync enquiryRepository, IMapper mapper)
        {
            _supplierRepository = enquiryRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateSupplierCommand command, CancellationToken cancellationToken)
        {
            var isExist = await _supplierRepository.GetByEmailAsync(command.Email);

            if (isExist != null)
            {
                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("", "Supplier with this email has already recorded. Please try another.")
                });
            }

            var supplier = _mapper.Map<Domain.Entities.Supplier>(command);
            await _supplierRepository.AddAsync(supplier);
            return new Response<int>(supplier.Id, "Supplier created successfully");
        }
    }
}