using Application.DTOs.Email;
using Application.Exceptions;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;


namespace Infrastructure.Shared.Services
{
    public class PdfService : IPdfService
    {
        private readonly Cloudinary _cloudinary;

        public PdfService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadPdfAsync(byte[] pdfBytes, string fileName, string folder)
        {
            using var stream = new MemoryStream(pdfBytes);
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, stream),
                PublicId = $"{folder}/{fileName}",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult?.SecureUrl?.ToString();
        }

        public byte[] GenerateAcquisitionPdf(Acquisition acquisition)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            using var stream = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Column(column =>
                    {
                        column.Item().Text("KEE WOODWORKING INDUSTRIES SDN BHD").Bold().FontSize(18);
                        column.Item().Text("No. 17, Jalan Wawasan 8, Kawasan Perindustrian Sri Gading, Johor, 83300 Batu Pahat");
                        column.Item().Text("Tel: +603-123 45678");
                        column.Item().Text("Email: info@keewood.com.my");
                        column.Item().Text(" ");
                        column.Item().Text(" ");
                        column.Item().Text("PURCHASE ORDER").Bold().FontSize(16).AlignCenter();
                        column.Item().Text(" ");
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text($"Issue Date: {DateTime.Now:yyyy-MM-dd}");
                        col.Item().Text($"Delivery Date: {DateTime.Now.AddDays(7):yyyy-MM-dd}");
                        col.Item().Text($"Deliver To: Kee Woodworking Warehouse");
                        col.Item().Text($"Attn: {acquisition.Supplier.ContactPerson ?? "N/A"}").Italic();
                        col.Item().Text(" ");

                        col.Item().PaddingTop(10).PaddingBottom(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("No.").Bold();
                                header.Cell().Text("Code").Bold();
                                header.Cell().Text("Name").Bold();
                                header.Cell().Text("Quantity").Bold();
                            });

                            int index = 1;
                            foreach (var item in acquisition.Items)
                            {
                                table.Cell().Text(index++.ToString());
                                table.Cell().Text(item.Inventory?.Code ?? "N/A");
                                table.Cell().Text(item.Inventory?.Name ?? "N/A");
                                table.Cell().Text(item.Quantity.ToString());
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }

        [Obsolete]
        public byte[] GenerateInvoicePdf(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            using var stream = new MemoryStream();

            var customerName = "LY FURNITURE SDN BHD";
            var customerAddress = "NO 15, JALAN WAWASAN UTAMA,\nKAWASAN PERINDUSTRIAN SRI GADING,\n83300 BATU PAHAT,\nJOHOR, MALAYSIA.";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // === Header ===
                    page.Header().Column(header =>
                    {
                        header.Item().Text("KEE WOODWORKING INDUSTRIES SDN BHD").Bold().FontSize(18);
                        header.Item().Text("No. 17, Jalan Wawasan 8, Kawasan Perindustrian Sri Gading, Johor, 83300 Batu Pahat");
                        header.Item().Text("Tel: +603-1234 5678");
                        header.Item().Text("Fax No: +607-455 7313");
                        header.Item().Text("Sales Tax ID No: J11-1808-21023021");
                        header.Item().Text("");
                        header.Item().Text("");
                    });

                    // === Invoice Info and Bill To ===
                    page.Content().Column(content =>
                    {
                        content.Item().Row(row =>
                        {
                            row.RelativeColumn().Column(col =>
                            {
                                col.Item().Text("Bill To:").Bold();
                                col.Item().Text(customerName);
                                col.Item().Text(customerAddress);
                                col.Item().Text("Tel: 607-455 8828  Fax: 607-4558802");
                            });

                            row.ConstantColumn(200).Column(col =>
                            {
                                col.Item().Text("INVOICE").Bold().FontSize(14);
                                col.Item().Text($"No.: INV{order.Id:D6}");
                                col.Item().Text($"Date: {DateTime.UtcNow:dd/MM/yyyy}");
                                col.Item().Text($"P/O Ref.: OPO{order.Id:D6}");
                                col.Item().Text("Terms: 14 days");
                                col.Item().Text("Page: 1");
                            });
                        });

                        // === Product Table ===
                        content.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);    // No.
                                columns.ConstantColumn(60);    // Model
                                columns.ConstantColumn(60);    // Code
                                columns.RelativeColumn(2);     // Description
                                columns.ConstantColumn(50);    // Qty
                                columns.ConstantColumn(80);    // Unit Price
                                columns.ConstantColumn(80);    // Amount
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Text("No.").Bold();
                                header.Cell().Text("Model").Bold();
                                header.Cell().Text("Code").Bold();
                                header.Cell().Text("Description").Bold();
                                header.Cell().Text("Qty").Bold().AlignRight();
                                header.Cell().Text("Unit Price").Bold().AlignRight();
                                header.Cell().Text("Amount").Bold().AlignRight();
                            });

                            int i = 1;

                            // Row 1: Product
                            table.Cell().Text(i++.ToString());
                            table.Cell().Text(order.Model ?? "-");
                            table.Cell().Text(order.ModelCode ?? "-");
                            table.Cell().Text(order.ModelCategory ?? "-");
                            table.Cell().Text($"{order.Quantity} PCS").AlignRight();
                            table.Cell().Text($"{order.UnitPrice:F4}").AlignRight();
                            table.Cell().Text($"{order.TotalPrice:F2}").AlignRight();

                            // Row 2: LESS MATERIAL SUPPLY
                            table.Cell().Text(i++.ToString());
                            table.Cell().Text("-");
                            table.Cell().Text("-");
                            table.Cell().Text("LESS: MATERIAL SUPPLY BY LY").Italic();
                            table.Cell().Text("-").AlignRight();
                            table.Cell().Text("-").AlignRight();
                            table.Cell().Text($"{order.MaterialCost:F2}").AlignRight();

                            // Row 3: WAGES
                            table.Cell().Text(i++.ToString());
                            table.Cell().Text("-");
                            table.Cell().Text("-");
                            table.Cell().Text("WAGES").Italic();
                            table.Cell().Text("-").AlignRight();
                            table.Cell().Text("-").AlignRight();
                            table.Cell().Text($"-{order.TotalRevenue:F2}").AlignRight();
                        });

                        // === Amount Summary (Right Aligned) ===
                        content.Item().PaddingTop(20).Row(row =>
                        {
                            // Left: Amount in words
                            row.RelativeColumn().Text($"RINGGIT M'SIA {NumberToWords(order.TotalRevenue)} ONLY").Bold();

                            // Right: Totals Table
                            row.ConstantColumn(220).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn();
                                    cols.ConstantColumn(80);
                                });

                                table.Cell().Text("Subtotal (Excl. Tax)").AlignRight();
                                table.Cell().Text($"{order.TotalRevenue:F2}").AlignRight();

                                table.Cell().Text("Sales Tax (0%)").AlignRight();
                                table.Cell().Text("0.00").AlignRight();

                                table.Cell().Text("Grand Total").Bold().AlignRight();
                                table.Cell().Text($"{order.TotalRevenue:F2}").Bold().AlignRight();
                            });
                        });

                        // === Footer Signature ===
                        content.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeColumn().Column(col =>
                            {
                                col.Item().Text("FOR KEE WOODWORKING INDUSTRIES SDN BHD").Bold();
                                col.Item().PaddingTop(20).Text("Kee Woodworking").Italic();
                            });

                            row.ConstantColumn(250).AlignRight().Text("CUSTOMER SIGNATURE & STAMP");
                        });
                    });
                });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }

        private string NumberToWords(decimal number)
        {
            if (number == 0)
                return "ZERO";

            var intPart = (int)Math.Floor(number);
            var cents = (int)((number - intPart) * 100);

            var words = $"{NumberToWords(intPart)} RINGGIT";
            if (cents > 0)
            {
                words += $" AND {NumberToWords(cents)} CENTS";
            }

            return words;
        }

        private string NumberToWords(int number)
        {
            if (number == 0)
                return "";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string[] unitsMap = {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX",
            "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE",
            "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN",
            "EIGHTEEN", "NINETEEN"
        };

            string[] tensMap = {
            "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY",
            "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
        };

            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += NumberToWords(number / 1000000000) + " BILLION ";
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " MILLION ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " HUNDRED ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "AND ";

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }
}
