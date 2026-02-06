using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ShopEaseApp.Services;

public class CartService
{
    private const string StorageKey = "shopease.cart";
    private readonly List<Product> _items = new();
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;

    public CartService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public event Action? OnChange;

    public IReadOnlyList<Product> Items => _items;

    public decimal Total => _items.Sum(item => item.Price);

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        var payload = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
        if (!string.IsNullOrWhiteSpace(payload))
        {
            var items = JsonSerializer.Deserialize<List<Product>>(payload);
            if (items != null)
            {
                _items.Clear();
                _items.AddRange(items);
            }
        }

        _isInitialized = true;
        NotifyStateChanged();
    }

    public async Task<bool> AddProductAsync(Product product)
    {
        if (product == null)
        {
            return false;
        }

        product.SanitizeFields();
        if (!product.TryValidate(out _))
        {
            return false;
        }

        _items.Add(product);
        await SaveAsync();
        NotifyStateChanged();
        return true;
    }

    public async Task<bool> RemoveProductAsync(int productId)
    {
        var index = _items.FindIndex(item => item.ProductID == productId);
        if (index < 0)
        {
            return false;
        }

        _items.RemoveAt(index);
        await SaveAsync();
        NotifyStateChanged();
        return true;
    }

    public async Task ClearAsync()
    {
        if (_items.Count == 0)
        {
            return;
        }

        _items.Clear();
        await SaveAsync();
        NotifyStateChanged();
    }

    private Task SaveAsync()
    {
        var payload = JsonSerializer.Serialize(_items);
        return _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, payload).AsTask();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
