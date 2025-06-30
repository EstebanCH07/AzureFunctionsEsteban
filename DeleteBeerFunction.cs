using AzureFunctionEsteban.Data;
using AzureFunctionEsteban.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFunctionEsteban;

public class DeleteBeerFunction
{
	private readonly BeerRepository _beerRepository;
	private readonly ILogger<DeleteBeerFunction> _logger;

	public DeleteBeerFunction(BeerRepository beerRepository, ILogger<DeleteBeerFunction> logger)
	{
		_beerRepository = beerRepository;
		_logger = logger;
	}

	[Function("DeleteBeer")]
	public async Task<HttpResponseData> Run(
		[HttpTrigger(AuthorizationLevel.Function, "delete", Route = "beer")] HttpRequestData req)
	{
		_logger.LogInformation($"Procesando solicitud para eliminar cerveza.");

		var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		var beer = JsonSerializer.Deserialize<Beer>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

		if (beer == null || beer.BeerId == 0)
		{
			var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
			await badResponse.WriteStringAsync("Cerveza inválida o ID no especificado.");
			return badResponse;
		}

		var rows = await _beerRepository.DeleteAsync(beer.BeerId);

		if (rows > 0)
		{
			var okResponse = req.CreateResponse(HttpStatusCode.OK);
			await okResponse.WriteStringAsync("Cerveza eliminada correctamente.");
			return okResponse;
		}
		else
		{
			var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
			await notFoundResponse.WriteStringAsync("No se encontró la cerveza para eliminar.");
			return notFoundResponse;
		}
	}
}