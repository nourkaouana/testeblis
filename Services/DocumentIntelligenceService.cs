using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;
using testbills.Data;
using testbills.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace testbills.Services
{
    public class DocumentIntelligenceService
    {
        private readonly DocumentAnalysisClient _client;
        private readonly ApplicationDbContext _db;

        public DocumentIntelligenceService(IConfiguration config, ApplicationDbContext db)
        {
            var endpoint = new Uri(config["AzureDocumentIntelligence:Endpoint"]);
            var credential = new AzureKeyCredential(config["AzureDocumentIntelligence:Key"]);
            _client = new DocumentAnalysisClient(endpoint, credential);
            _db = db;
        }

        public async Task<Dictionary<string, string>> AnalyzeInvoiceAsync(Stream imageStream)
        {
            var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", imageStream);
            var result = operation.Value;
            var output = new Dictionary<string, string>();

            var doc = result.Documents.FirstOrDefault();
            if (doc == null)
                return output;

            decimal subtotal = 0;

            if (doc.Fields.TryGetValue("SubTotal", out var subTotalField) && subTotalField.Value != null)
            {
                var currencyValue = subTotalField.Value.AsCurrency();
                subtotal = (decimal)currencyValue.Amount;
                output["SubTotal"] = subtotal.ToString("0.00");
            }

            if (doc.Fields.TryGetValue("InvoiceTotal", out var totalField) && totalField.Value != null)
            {
                var currencyValue = totalField.Value.AsCurrency();
                output["Total"] = ((decimal)currencyValue.Amount).ToString("0.00");
            }

            if (doc.Fields.TryGetValue("InvoiceDate", out var invoiceDateField) && invoiceDateField.Value != null)
            {
                var date = invoiceDateField.Value.AsDate();
                output["InvoiceDate"] = date.ToString("yyyy-MM-dd");
            }

            if (doc.Fields.TryGetValue("Description", out var descriptionField) && descriptionField.Value != null)
            {
                output["Description"] = descriptionField.Content ?? descriptionField.Value.ToString();
            }

            if (doc.Fields.TryGetValue("TaxDetails", out var taxDetailsField) && taxDetailsField.Value != null)
            {
                var taxList = taxDetailsField.Value.AsList();
                int i = 1;

                foreach (var taxItem in taxList)
                {
                    if (taxItem.Value != null)
                    {
                        var taxObject = taxItem.Value.AsDictionary();
                        decimal? taxAmount = null;
                        decimal? taxRate = null;

                        if (taxObject.TryGetValue("Amount", out var taxAmountField) && taxAmountField.Value != null)
                        {
                            var amountValue = taxAmountField.Value.AsCurrency();
                            taxAmount = (decimal)amountValue.Amount;
                            output[$"TaxAmount_{i}"] = taxAmount.Value.ToString("0.00");
                        }

                        if (taxObject.TryGetValue("TaxRate", out var taxRateField) && taxRateField.Value != null)
                        {
                            var rateValue = taxRateField.Value.AsDouble();
                            taxRate = Convert.ToDecimal(rateValue);
                            output[$"TaxRate_{i}"] = taxRate.Value.ToString("0.00") + "%";
                        }
                        else if (taxAmount.HasValue && subtotal > 0)
                        {
                            var inferred = (taxAmount.Value / subtotal) * 100;
                            output[$"TaxRate_{i}"] = inferred.ToString("0.00") + "% (computed)";
                        }

                        i++;
                    }
                }
            }

            try
            {
                // Create invoice data
                var invoice = new InvoiceData
                {
                    Libelle = output.TryGetValue("Description", out var libelle) ? libelle : null,
                    Date = output.TryGetValue("InvoiceDate", out var invoiceDate) ? invoiceDate : null,
                    Montant_HT = output.TryGetValue("SubTotal", out var ht) ? ht : null,
                    Montant_TTC = output.TryGetValue("Total", out var ttc) ? ttc : null,
                    TVA = output.FirstOrDefault(x => x.Key.StartsWith("TaxAmount_")).Value
                };

                // Add tax details
                int j = 1;
                while (true)
                {
                    bool hasTaxAmount = output.TryGetValue($"TaxAmount_{j}", out var taxAmount);
                    bool hasTaxRate = output.TryGetValue($"TaxRate_{j}", out var taxRate);

                    if (!hasTaxAmount && !hasTaxRate)
                        break;

                    invoice.TaxDetails.Add(new TaxDetail
                    {
                        TaxAmount = hasTaxAmount ? taxAmount : null,
                        TaxRate = hasTaxRate ? taxRate : null
                    });

                    j++;
                }

                // Use a transaction for database operations
                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    _db.Invoices.Add(invoice);
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it to the client
                // You might want to add proper logging here
                Console.WriteLine($"Error saving invoice data: {ex.Message}");
            }

            return output;
        }

        public async Task<Dictionary<string, string>> DumpAllFieldsAsync(Stream imageStream)
        {
            var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", imageStream);
            var result = operation.Value;
            var output = new Dictionary<string, string>();

            var doc = result.Documents.FirstOrDefault();
            if (doc == null)
                return output;

            foreach (var field in doc.Fields)
            {
                var fieldName = field.Key;
                var rawValue = field.Value.Content ?? field.Value.ToString();
                output[fieldName] = rawValue;
            }

            return output;
        }

        public async Task<Dictionary<string, string>> ExtractFilteredFieldsAsync(Stream imageStream)
        {
            var fullResult = await AnalyzeInvoiceAsync(imageStream);
            var filteredFields = new Dictionary<string, string>();

            if (fullResult.TryGetValue("Description", out var label) && !string.IsNullOrWhiteSpace(label))
                filteredFields["Libelle"] = label;

            if (fullResult.TryGetValue("InvoiceDate", out var date))
                filteredFields["Date"] = date;

            if (fullResult.TryGetValue("SubTotal", out var ht))
                filteredFields["Montant_HT"] = ht;

            if (fullResult.TryGetValue("Total", out var ttc))
                filteredFields["Montant_TTC"] = ttc;

            var taxAmount = fullResult.FirstOrDefault(kvp => kvp.Key.StartsWith("TaxAmount_", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(taxAmount.Value))
                filteredFields["TVA"] = taxAmount.Value;

            return filteredFields;
        }
    }
}
