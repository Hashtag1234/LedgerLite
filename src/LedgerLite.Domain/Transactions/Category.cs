namespace LedgerLite.Domain.Transactions;

// WHY: Category is modeled as a Value Object to enforce validation on the name and enable value-based comparison without needing an database-backed identity at this layer.
public readonly record struct Category
{
    public string Name { get; }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
    }

    public override string ToString()
    {
        return Name;
    }
}
