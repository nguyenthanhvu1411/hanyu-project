using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Operations;

public sealed class SystemSetting : TimestampedEntity
{
    public string Key { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Group { get; private set; } = "General";
    public string Value { get; private set; } = string.Empty;
    public string ValueType { get; private set; } = "string";
    public string? Description { get; private set; }

    protected SystemSetting() { }

    public SystemSetting(
        string key,
        string displayName,
        string group,
        string value,
        string valueType,
        string? description = null)
    {
        UpdateMetadata(key, displayName, group, valueType, description);
        UpdateValue(value);
    }

    public void UpdateMetadata(
        string key,
        string displayName,
        string group,
        string valueType,
        string? description)
    {
        key = NormalizeRequired(key, nameof(key), 120).ToLowerInvariant();
        displayName = NormalizeRequired(displayName, nameof(displayName), 160);
        group = NormalizeRequired(group, nameof(group), 80);
        valueType = NormalizeRequired(valueType, nameof(valueType), 20).ToLowerInvariant();

        if (valueType is not ("string" or "number" or "boolean" or "json"))
            throw new ArgumentException("ValueType chỉ hỗ trợ string, number, boolean hoặc json.", nameof(valueType));

        if (description?.Length > 500)
            throw new ArgumentException("Description không được vượt quá 500 ký tự.", nameof(description));

        Key = key;
        DisplayName = displayName;
        Group = group;
        ValueType = valueType;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        MarkUpdated();
    }

    public void UpdateValue(string value)
    {
        value ??= string.Empty;
        if (value.Length > 8000)
            throw new ArgumentException("Value không được vượt quá 8000 ký tự.", nameof(value));

        ValidateValue(ValueType, value);
        Value = value;
        MarkUpdated();
    }

    private static void ValidateValue(string valueType, string value)
    {
        if (valueType == "number" && !decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _))
            throw new ArgumentException("Giá trị number không hợp lệ.", nameof(value));

        if (valueType == "boolean" && !bool.TryParse(value, out _))
            throw new ArgumentException("Giá trị boolean phải là true hoặc false.", nameof(value));

        if (valueType == "json")
        {
            try { System.Text.Json.JsonDocument.Parse(value); }
            catch (System.Text.Json.JsonException) { throw new ArgumentException("Giá trị JSON không hợp lệ.", nameof(value)); }
        }
    }

    private static string NormalizeRequired(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{parameterName} không được để trống.", parameterName);
        value = value.Trim();
        if (value.Length > maxLength)
            throw new ArgumentException($"{parameterName} không được vượt quá {maxLength} ký tự.", parameterName);
        return value;
    }
}
