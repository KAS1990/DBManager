namespace DBManager.Scanning
{
    public interface ICanRefreshFrom
    {
        void RefreshFrom(ICanRefreshFrom rhs,
                        bool SkipNullsForObjects,
                        bool SkipNullsForNullables);
    }
}
