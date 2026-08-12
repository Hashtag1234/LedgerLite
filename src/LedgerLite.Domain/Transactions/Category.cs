namespace LedgerLite.Domain.Transactions;

// WHY: Category is a value object that validates its name
// and supports value-based comparison without requiring a database-backed identity.
public record Category
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
