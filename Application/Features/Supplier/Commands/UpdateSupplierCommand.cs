using Application.DTOs.User;
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
    public class UpdateSupplierCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPerson { get; set; }
    }

    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Response<int>>
    {
        private readonly ISupplierRepositoryAsync _supplierRepository;
        private readonly IMapper _mapper;

        public UpdateSupplierCommandHandler(ISupplierRepositoryAsync supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.Id);

            if (supplier == null)
            {
                throw new ApiException($"Supplier Not Found.");
            }

            if (!string.Equals(supplier.Email, request.Email, System.StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _supplierRepository.GetByEmailAsync(request.Email);
                if (existing != null && existing.Id != supplier.Id)
                {
                    throw new ValidationException(new List<ValidationFailure>
                    {
                        new ValidationFailure("", "Supplier with this email has already recorded. Please try another.")
                    });
                }
            }

            _mapper.Map(request, supplier);
            await _supplierRepository.UpdateAsync(supplier);

            return new Response<int>(supplier.Id, "Supplier updated successfully");
        }
    }
}