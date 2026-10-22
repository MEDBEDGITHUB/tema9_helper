using System.Globalization;

namespace FlowchartExplainer.Models;

/// <summary>
/// Строго типизированное значение переменной алгоритма.
/// Поддерживает числа, строки и булевы флаги с форматированием.
/// </summary>
public readonly struct VariableValue : IEquatable<VariableValue>
{
    public double? AsDouble { get; }
    public string? AsString { get; }
    public bool? AsBool { get; }

    public bool IsNumber => AsDouble.HasValue;
    public bool IsBool => AsBool.HasValue;
    public bool IsString => AsString != null && !AsBool.HasValue;
    public bool IsEmpty => !AsDouble.HasValue && AsString == null && !AsBool.HasValue;

    public VariableValue(double value)
    {
        AsDouble = value;
        AsString = null;
        AsBool = null;
    }

    public VariableValue(int value)
    {
        AsDouble = value;
        AsString = null;
        AsBool = null;
    }

    public VariableValue(string? value)
    {
        AsDouble = null;
        AsString = value;
        AsBool = null;
    }

    public VariableValue(bool value)
    {
        AsDouble = null;
        AsString = null;
        AsBool = value;
    }

    public static VariableValue Empty => default;

    public static implicit operator VariableValue(double value) => new(value);
    public static implicit operator VariableValue(int value) => new(value);
    public static implicit operator VariableValue(string? value) => new(value);
    public static implicit operator VariableValue(bool value) => new(value);

    public double ToDouble() => AsDouble ?? (double.TryParse(AsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0);
    public int ToInt() => (int)Math.Round(ToDouble());

    public override string ToString()
    {
        if (IsEmpty) return "—";
        if (AsBool.HasValue) return AsBool.Value ? "Да (True)" : "Нет (False)";
        if (AsDouble.HasValue)
        {
            var val = AsDouble.Value;
            if (Math.Abs(val - Math.Round(val)) < 0.000001)
                return Math.Round(val).ToString(CultureInfo.InvariantCulture);
            return val.ToString("0.##", CultureInfo.InvariantCulture);
        }
        return AsString ?? "—";
    }

    public string TypeName
    {
        get
        {
            if (IsEmpty) return "не определен";
            if (AsBool.HasValue) return "bool";
            if (AsDouble.HasValue)
            {
                var val = AsDouble.Value;
                return Math.Abs(val - Math.Round(val)) < 0.000001 ? "int" : "decimal";
            }
            return "string";
        }
    }

    public bool Equals(VariableValue other)
    {
        if (IsEmpty && other.IsEmpty) return true;
        if (AsBool.HasValue && other.AsBool.HasValue) return AsBool == other.AsBool;
        if (AsDouble.HasValue && other.AsDouble.HasValue) return Math.Abs(AsDouble.Value - other.AsDouble.Value) < 0.000001;
        return string.Equals(AsString, other.AsString, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => obj is VariableValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(AsDouble, AsString, AsBool);

    public static bool operator ==(VariableValue left, VariableValue right) => left.Equals(right);
    public static bool operator !=(VariableValue left, VariableValue right) => !left.Equals(right);
}
