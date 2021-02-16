public static class RomanNumerals
{
    public static readonly string[] numerals = {"I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"}; 
    public static string convert(int number)
    {
        if (1 <= number && number <= 10)
            return numerals[number - 1];
        else
            return "?";
    }
}
