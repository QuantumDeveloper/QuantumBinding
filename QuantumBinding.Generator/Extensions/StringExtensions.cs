using System.Text;

namespace QuantumBinding.Generator.Extensions;

public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (i > 0)
            {
                char prev = input[i - 1];

                if ((char.IsLower(prev) && char.IsUpper(c)) ||
                    (char.IsDigit(prev) && char.IsUpper(c)) ||
                    (char.IsLetter(prev) && char.IsDigit(c)))
                {
                    sb.Append('_');
                }
                else if (i < input.Length - 1 && char.IsUpper(prev) && char.IsUpper(c) && char.IsLower(input[i + 1]))
                {
                    sb.Append('_');
                }
            }

            sb.Append(char.ToUpper(c));
        }

        string result = sb.ToString();

        if (result.StartsWith("VK_") == false && result.StartsWith("VK"))
        {
            if (result.StartsWith("V_K"))
                result = "VK_" + result.Substring(3);
            else
                result = "VK_" + result.Substring(2);
        }

        return result;
    }
}