using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Acquisition.Commands
{
    public class GenerateAcquisitionPdfCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class GenerateAcquisitionPdfCommandHandler : IRequestHandler<GenerateAcquisitionPdfCommand, Response<int>>
    {
        private readonly IAcquisitionRepositoryAsync _acquisitionRepository;
        private readonly IPdfService _pdfService;
        public GenerateAcquisitionPdfCommandHandler(IAcquisitionRepositoryAsync acquisitionRepository, IAcquisitionItemRepositoryAsync acquisitionItemRepository, IPdfService pdfService)
        {
            _acquisitionRepository = acquisitionRepository;
            _pdfService = pdfService;
        }
        public async Task<Response<int>> Handle(GenerateAcquisitionPdfCommand command, CancellationToken cancellationToken)
        {
            var acquisition = await _acquisitionRepository.GetByIdAsync(command.Id);

            if (acquisition == null)
            {
                throw new ApiException("Acquisition not found.");
            }            
            

            var pdfBytes = _pdfService.GenerateAcquisitionPdf(acquisition);
            var fileName = $"AC{acquisition.Id:D4}_{DateTime.UtcNow:yyyyMMdd}.pdf";

            var cloudUrl = await _pdfService.UploadPdfAsync(pdfBytes, fileName, "acquisitions");
            acquisition.filePath = cloudUrl;

            await _acquisitionRepository.UpdateAsync(acquisition);
            
            return new Response<int>(acquisition.Id, cloudUrl);
        }
    }
}
