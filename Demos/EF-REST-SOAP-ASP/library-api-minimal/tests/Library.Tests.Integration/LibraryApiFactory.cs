using Library.ControllerApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Tests.Integration;

public class LibraryApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<ISupplierClient, FakeSupplierClient>();
        });
    }
}

public class FakeSupplierClient : ISupplierClient
{
    public Task<decimal?> GetListPriceAsync(string sku)
    {
        return Task.FromResult<decimal?>(99.99m);
    }
}