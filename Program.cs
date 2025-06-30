using AzureFunctionEsteban.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(services =>
	{
		services.AddSingleton<SqlConnectionFactory>();
		services.AddScoped<BeerRepository>();
	})
	.Build();

host.Run();
