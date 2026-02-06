namespace ShopEaseApp;

public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Price { get; set; }
    public string Category { get; set; } = string.Empty;

    public void PrintDetails()
    {
        Console.WriteLine($"Product: {Name} | Price: ${Price:0.00} | Category: {Category}");
    }
}
