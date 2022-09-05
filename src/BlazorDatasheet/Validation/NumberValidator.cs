using BlazorDatasheet.Interfaces;

namespace BlazorDatasheet.Validation;

public class NumberValidator : IDataValidator
{
    public NumberValidator(bool isStrict)
    {
        IsStrict = isStrict;
    }

    public bool IsValid(object value)
    {
        var val = value.ToString();
        return double.TryParse(val, out double res);
    }

    public bool IsStrict { get; private set; }
}