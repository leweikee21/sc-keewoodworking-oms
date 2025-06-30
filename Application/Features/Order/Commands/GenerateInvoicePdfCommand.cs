using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Order.Commands
{
    public class GenerateInvoicePdfCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class GenerateInvoicePdfCommandHandler : IRequestHandler<GenerateInvoicePdfCommand, Response<int>>
    {
        private readonly IOrderRepositoryAsync _orderRepository;
        private readonly IPdfService _pdfService;
        public GenerateInvoicePdfCommandHandler(IOrderRepositoryAsync orderRepository, IPdfService pdfService)
        {
            _orderRepository = orderRepository;
            _pdfService = pdfService;
        }
        public async Task<Response<int>> Handle(GenerateInvoicePdfCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(command.Id);

            if (order == null)
            {
                throw new ApiException("Order not found.");
            }            
            

            var pdfBytes = _pdfService.GenerateInvoicePdf(order);
            var fileName = $"INV_OPO{order.Id:D6}_{DateTime.UtcNow:yyyyMMdd}.pdf";

            var cloudUrl = await _pdfService.UploadPdfAsync(pdfBytes, fileName, "invoices");
            order.filePath = cloudUrl;
            await _orderRepository.UpdateAsync(order);

            return new Response<int>(order.Id, cloudUrl);
        }
    }
}
