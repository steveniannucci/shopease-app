namespace ShopEaseApp;

public class Product
{
    public const int MaxNameLength = 100;
    public const int MaxCategoryLength = 50;

    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;

    public void PrintDetails()
    {
        Console.WriteLine($"Product: {Name} | Price: ${Price:0.00} | Category: {Category}");
    }

    public void SanitizeFields()
    {
        Name = SanitizeText(Name);
        Category = SanitizeText(Category);
    }

    public bool TryValidate(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            errorMessage = "Name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Category))
        {
            errorMessage = "Category is required.";
            return false;
        }

        if (Name.Length > MaxNameLength)
        {
            errorMessage = $"Name must be {MaxNameLength} characters or less.";
            return false;
        }

        if (Category.Length > MaxCategoryLength)
        {
            errorMessage = $"Category must be {MaxCategoryLength} characters or less.";
            return false;
        }

        if (Price < 0m)
        {
            errorMessage = "Price must be 0 or greater.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private static string SanitizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        return trimmed.Replace("<", string.Empty).Replace(">", string.Empty);
    }
}
