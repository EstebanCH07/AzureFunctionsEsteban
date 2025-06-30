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

public class CreateBeerFunction
{
	private readonly BeerRepository _beerRepository;
	private readonly ILogger<CreateBeerFunction> _logger;

	public CreateBeerFunction(BeerRepository beerRepository, ILogger<CreateBeerFunction> logger)
	{
		_beerRepository = beerRepository;
		_logger = logger;
	}

	[Function("CreateBeer")]
	public async Task<HttpResponseData> Run(
		[HttpTrigger(AuthorizationLevel.Function, "post", Route = "beer")] HttpRequestData req)
	{
		_logger.LogInformation("Procesando solicitud para crear cerveza.");

		var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		var beer = JsonSerializer.Deserialize<Beer>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

		if (beer == null)
		{
			var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
			await badResponse.WriteStringAsync("El cuerpo de la solicitud es inválido.");
			return badResponse;
		}

		var rows = await _beerRepository.CreateAsync(beer);

		if (rows > 0)
		{
			var okResponse = req.CreateResponse(HttpStatusCode.Created);
			await okResponse.WriteStringAsync("Cerveza creada correctamente.");
			return okResponse;
		}
		else
		{
			var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
			await errorResponse.WriteStringAsync("Error al crear la cerveza.");
			return errorResponse;
		}
	}
}