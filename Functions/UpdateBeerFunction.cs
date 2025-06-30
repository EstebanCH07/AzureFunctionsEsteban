using AzureFunctionEsteban.Data;
using AzureFunctionEsteban.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctionEsteban.Functions;

public class UpdateBeerFunction
{
    private readonly BeerRepository _beerRepository;
    private readonly ILogger<UpdateBeerFunction> _logger;

    public UpdateBeerFunction(BeerRepository beerRepository, ILogger<UpdateBeerFunction> logger)
    {
        _beerRepository = beerRepository;
        _logger = logger;
    }

    [Function("UpdateBeer")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "beer")] HttpRequestData req)
    {
        _logger.LogInformation($"Procesando solicitud para actualizar cerveza.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var updatedBeer = JsonSerializer.Deserialize<Beer>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (updatedBeer == null || updatedBeer.BeerId == 0)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Cerveza inválida o ID no especificado.");
            return badResponse;
        }

        var rows = await _beerRepository.UpdateAsync(updatedBeer);

        if (rows > 0)
        {
            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            await okResponse.WriteStringAsync("Cerveza actualizada correctamente.");
            return okResponse;
        }
        else
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await errorResponse.WriteStringAsync("No se encontró la cerveza para actualizar.");
            return errorResponse;
        }
    }
}