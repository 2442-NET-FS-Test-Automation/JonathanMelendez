namespace DarkKitchen.Api.Fulfillment;

public interface IFulfillmentService
{
    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
}

public enum FulfillmentResult { Fulfilled, Backordered } 
public record BurstResult(int Fulfilled, int Backordered);
public class FulfillmentService : IFulfillmentService
{
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<FulfillmentResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}