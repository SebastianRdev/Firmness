        {
            // 1. Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(createDto.CustomerId);
            if (customer == null)
            {
                return ResultOft<SaleResponseDto>.Failure("Customer not found");
            }

            // 2. Validate inventory and get products
            var validationResult = await ValidateInventoryAsync(createDto.SaleDetails);
            if (!validationResult.IsSuccess)
            {
                return ResultOft<SaleResponseDto>.Failure(validationResult.ErrorMessage);
            }

            // 3. Create Sale entity
            var sale = new Sale
            {
                CustomerId = createDto.CustomerId,
                Date = createDto.Date,
                TotalAmount = createDto.TotalAmount,
                TaxAmount = createDto.TaxAmount,
                GrandTotal = createDto.GrandTotal,
                DeliveryFees = 0, // Can be added later
                SaleDetails = new List<SaleDetail>()
            };

            // 4. Add sale details and update inventory
            foreach (var detailDto in createDto.SaleDetails)
            {
                var product = await _productRepository.GetByIdAsync(detailDto.ProductId);
                if (product == null) continue;

                // Update product stock
                product.Stock -= detailDto.Quantity;
                await _productRepository.UpdateAsync(product);

                sale.SaleDetails.Add(new SaleDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    Product = product
                });
            }

            // 5. Save sale
            await _saleRepository.AddAsync(sale);

            // 6. Create receipt
            var receipt = new Receipt
            {
                SaleId = sale.Id,
                ReceiptNumber = GenerateReceiptNumber(sale.Id),
                IssueDate = DateTime.Now,
                FileName = $"Receipt_{sale.Id}.pdf"
            };

            await _receiptRepository.AddAsync(receipt);
            sale.Receipt = receipt;

            // 7. Generate PDF
            var pdfPath = Path.Combine(_environment.WebRootPath, "receipts", receipt.FileName);
            var pdfGenerated = await _pdfService.GeneratePdfAsync(sale, pdfPath);

            if (!pdfGenerated)
            {
                _logger.LogWarning("PDF generation failed for sale {SaleId}", sale.Id);
            }

            // 8. Send emails (to customer and admin)
            try
            {
                var emailBody = GenerateEmailBody(sale, customer);
                var recipients = new List<string> { customer.Email };
                
                if (!string.IsNullOrEmpty(_emailSettings.AdminEmail))
                {
                    recipients.Add(_emailSettings.AdminEmail);
                }

                await _emailService.SendEmailToMultipleAsync(
                    recipients,
                    $"Comprobante de Venta #{sale.Id}",
                    emailBody,
                    pdfPath,
                    receipt.FileName
                );

                _logger.LogInformation("Receipt email sent successfully for sale {SaleId}", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send receipt email for sale {SaleId}", sale.Id);
                // Don't fail the entire operation if email fails
            }

            // 9. Return response
            var response = new SaleResponseDto
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                CustomerName = customer.FullName,
                CustomerEmail = customer.Email,
                Date = sale.Date,
                TotalAmount = sale.TotalAmount,
                TaxAmount = sale.TaxAmount,
                GrandTotal = sale.GrandTotal,
                ReceiptFileName = receipt.FileName,
                SaleDetails = sale.SaleDetails.Select(d => new SaleDetailDto
                {
                    Id = d.Id,
                    ProductName = d.Product?.Name ?? "Unknown",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };

            return ResultOft<SaleResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale with receipt");
            return ResultOft<SaleResponseDto>.Failure($"Error creating sale: {ex.Message}");
        }
    }

    public async Task<ResultOft<SaleResponseDto>> GetSaleByIdAsync(int id)
    {
        try
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
            {
                return ResultOft<SaleResponseDto>.Failure("Sale not found");
            }

            var response = new SaleResponseDto
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.FullName ?? "Unknown",
                CustomerEmail = sale.Customer?.Email ?? "Unknown",
                Date = sale.Date,
                TotalAmount = sale.TotalAmount,
                TaxAmount = sale.TaxAmount,
                GrandTotal = sale.GrandTotal,
                ReceiptFileName = sale.Receipt?.FileName,
                SaleDetails = sale.SaleDetails?.Select(d => new SaleDetailDto
                {
                    Id = d.Id,
                    ProductName = d.Product?.Name ?? "Unknown",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList() ?? new List<SaleDetailDto>()
            };

            return ResultOft<SaleResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sale {SaleId}", id);
            return ResultOft<SaleResponseDto>.Failure($"Error retrieving sale: {ex.Message}");
        }
    }

    private async Task<Result> ValidateInventoryAsync(List<CreateSaleDetailDto> saleDetails)
    {
        foreach (var detail in saleDetails)
        {
            var product = await _productRepository.GetByIdAsync(detail.ProductId);
            
            if (product == null)
            {
                return Result.Failure($"Product with ID {detail.ProductId} not found");
            }

            if (product.Stock < detail.Quantity)
            {
                return Result.Failure($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}, Requested: {detail.Quantity}");
            }
        }

        return Result.Success();
    }

    private string GenerateReceiptNumber(int saleId)
    {
        var date = DateTime.Now;
        return $"REC-{date:yyyyMMdd}-{saleId:D6}";
    }

    private string GenerateEmailBody(Sale sale, Customer customer)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #1890ff; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f5f7fa; }}
                    .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                    .highlight {{ color: #1890ff; font-weight: bold; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>FIRMNESS - ConstructGo</h1>
                        <p>Comprobante de Venta</p>
                    </div>
                    <div class='content'>
                        <h2>Estimado/a {customer.FullName},</h2>
                        <p>Gracias por su compra. Adjunto encontrará el comprobante de su venta.</p>
                        
                        <p><strong>Detalles de la venta:</strong></p>
                        <ul>
                            <li>Número de venta: <span class='highlight'>#{sale.Id}</span></li>
                            <li>Fecha: <span class='highlight'>{sale.Date:dd/MM/yyyy HH:mm}</span></li>
                            <li>Total: <span class='highlight'>${sale.GrandTotal:N2}</span></li>
                        </ul>
                        
                        <p>Si tiene alguna pregunta, no dude en contactarnos.</p>
                    </div>
                    <div class='footer'>
                        <p>Este es un correo automático, por favor no responder.</p>
                        <p>&copy; {DateTime.Now.Year} FIRMNESS - ConstructGo. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }
}