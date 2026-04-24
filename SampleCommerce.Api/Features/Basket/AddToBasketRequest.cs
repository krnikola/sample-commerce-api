namespace SampleCommerce.Api.Features.Basket;

public class AddToBasketRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
