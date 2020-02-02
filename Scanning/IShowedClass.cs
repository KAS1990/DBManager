using DBManager.Scanning.DBAdditionalDataClasses;

namespace DBManager.Scanning
{
    public interface IShowedClass
    {
        #region OnStyleChanged and OnStyleChanged event
        event StyleChangedEventHandler StyleChanged;


        void OnStyleChanged(IShowedClass source, string propertyName);

        void OnStyleChanged(StyleChangedEventArgs e);
        #endregion
    }
}
