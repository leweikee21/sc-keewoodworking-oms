using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPdfService
    {
        Task<string> UploadPdfAsync(byte[] pdfBytes, string fileName, string folder);
        byte[] GenerateAcquisitionPdf(Acquisition acquisition);
        byte[] GenerateInvoicePdf(Order order);
        
    }
}
