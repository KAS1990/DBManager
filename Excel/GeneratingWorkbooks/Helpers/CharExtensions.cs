namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class CharExtensions
    {
        public static bool IsCapitalLatinLetter(this char ch)
        {
            return (ch >= 'A' && ch <= 'Z');
        }

        public static bool IsLowerCaseLatinLetter(this char ch)
        {
            return (ch >= 'a' && ch <= 'z');
        }

        public static bool IsLatinLetter(this char ch)
        {
            return ch.IsCapitalLatinLetter() || ch.IsLowerCaseLatinLetter();
        }

        public static char ToCapitalLatinLetter(this char ch)
        {
            if (ch.IsCapitalLatinLetter())
                return ch;
            else if (ch.IsLowerCaseLatinLetter())
                return (char)('A' + ('a' - ch));
            else
                return ch;
        }
    }
}
