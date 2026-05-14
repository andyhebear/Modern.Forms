namespace Modern.Forms.Layout;

internal sealed class SourceGenerated
{
    internal sealed class EnumValidator
    {
        public static void Validate (Enum value)
        {
            if (!Enum.IsDefined (value.GetType (), value))
                throw new ArgumentOutOfRangeException (value.GetType ().Name, value, $"The value '{value}' is not valid for '{value.GetType ().Name}'.");
        }
    }
}
