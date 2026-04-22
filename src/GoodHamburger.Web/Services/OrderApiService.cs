using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class OrderApiService(HttpClient http)
{
    public Task<List<MenuItemModel>?> GetMenuAsync() =>
        http.GetFromJsonAsync<List<MenuItemModel>>("/menu");

    public Task<List<OrderResponse>?> GetOrdersAsync() =>
        http.GetFromJsonAsync<List<OrderResponse>>("/orders");

    public Task<OrderResponse?> GetOrderAsync(Guid id) =>
        http.GetFromJsonAsync<OrderResponse>($"/orders/{id}");

    public async Task<OrderResponse?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await http.PostAsJsonAsync("/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        var response = await http.DeleteAsync($"/orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}
