namespace HesapKitap
{
    internal static class StringHelper
    {
        internal static bool TryParse(string input, out string output)
        {
            output = string.Format("{0}", input).Trim();

            if (string.IsNullOrWhiteSpace(output))
                return false;

            return true;
        }
    }
}