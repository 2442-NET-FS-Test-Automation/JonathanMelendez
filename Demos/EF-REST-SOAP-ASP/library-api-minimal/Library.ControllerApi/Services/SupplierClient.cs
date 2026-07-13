namespace Library.ControllerApi.Services;

public class SupplierClient(HttpClient http) : ISupplierClient
{
    private readonly HttpClient _http = http;
    private record SupplierProduct(int Id, string Title, decimal Price);
    public async Task<decimal?> GetListPriceAsync(string sku)
    {
        var digits = sku.Split('-')[1]; // assuming number is in second part
        if (!int.TryParse(digits, out var id)) return null;
        
        var product = await _http.GetFromJsonAsync<SupplierProduct>($"/product/{id}");
        
        return product?.Price;
    }
}