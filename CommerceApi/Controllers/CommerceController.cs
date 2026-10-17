using CommerceApi.Dtos;
using CommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommerceController : ControllerBase
    {
        private readonly ICommerceService _commerceService;

        public CommerceController(ICommerceService commerceService)
        {
            _commerceService = commerceService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "El archivo (.csv) se encuentra vacío o no fue recibido correctamente." });
                }

                int processedCount = await _commerceService.ProcessCsvFileAsync(file);

                return Ok(new
                {
                    message = "Archivo cargado y registrado en BD exitosamente.",
                    totalRecords = processedCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al procesar el archivo CSV", errorDetail = ex.Message });
            }
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessCommerce([FromBody] ProcessDateRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                int quarantineCount = await _commerceService.ProcessCommerceByDateAsync(request.ProcessDate);

                return Ok(new
                {
                    recordsInQuarantine = quarantineCount,
                    message = $"Procesamiento finalizado. {quarantineCount} registros movidos a cuarentena."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al procesar fecha", errorDetail = ex.Message });
            }
        }

        [HttpGet("quarantine")]
        public async Task<IActionResult> GetQuarantineList()
        {
            try
            {
                var result = await _commerceService.GetQuarantineListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al obtener lista de cuarentena", errorDetail = ex.Message });
            }
        }
    }
}