using AzureFunctionEsteban.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace AzureFunctionEsteban.Functions
{
	public class GetBeersFunction
	{
		private readonly ILogger<GetBeersFunction> _logger;
		private readonly BeerRepository _beerRepository;


		public GetBeersFunction(ILogger<GetBeersFunction> logger, BeerRepository beerRepository)
		{
			_logger = logger;
			_beerRepository = beerRepository;
		}

		[Function("GetBeers")]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "beers")] HttpRequestData req)
		{
			_logger.LogInformation("Obteniendo la lista de cervezas...");

			var beers = await _beerRepository.GetAllAsync();

			var response = req.CreateResponse(HttpStatusCode.OK);
			await response.WriteAsJsonAsync(beers);

			return response;
		}
	}
}
