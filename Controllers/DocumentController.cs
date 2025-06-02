using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testbills.Services;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DocumentController : ControllerBase
{
    private readonly DocumentIntelligenceService _documentService;

    public DocumentController(DocumentIntelligenceService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("analyze")]
   // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AnalyzeReceipt(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var result = await _documentService.AnalyzeInvoiceAsync(stream);
        return Ok(result);
    }
    [HttpPost("RawaAnalyze")]
    // [Authorize(Roles = "user")]
    public async Task<IActionResult> AnalyzeInvoice(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var fields = await _documentService.DumpAllFieldsAsync(stream);

        return Ok(fields);
    }
    [HttpPost("analyze/filtered")]
    public async Task<IActionResult> AnalyzeInvoiceFiltered(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var filtered = await _documentService.ExtractFilteredFieldsAsync(stream);
        return Ok(filtered);
    }


}
    

   












