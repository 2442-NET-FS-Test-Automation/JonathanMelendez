namespace Library.Api.Fulfillment;

public class UnknownSkuException (string sku) : Exception($"Unknown SKU: {sku}")
{
    public string Sku { get; } = sku;
}