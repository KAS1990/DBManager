using System.Diagnostics;
using System.Windows.Controls;

namespace DBManager.InterfaceElements
{
    /// <summary>
    /// Кнопка "Отмена".
    /// Этот класс нужен просто чтобы отличить кнопку "Отмена"/"Закрыть" от других на форме
    /// </summary>
    [DebuggerDisplay("Text = {Content}")]
    public class CancelButton : Button, IVisBaseElement
    {
        #region Свойство FieldName
        /// <summary>
        /// Это свойство в этом классе не используется
        /// </summary>
        public string FieldName
        {
            get { return ""; }
            set { }
        }
        #endregion


        #region Свойство ElementCoords
        private PointI m_ElementCoords = new PointI(-1, -1);

        /// <summary>
        /// Координаты елемента, если он используется в ListBox
        /// </summary>
        public PointI ElementCoords
        {
            get { return m_ElementCoords; }
            set { m_ElementCoords = value; }
        }
        #endregion


        public CancelButton()
        {
            Padding = new System.Windows.Thickness(3, 0, 3, 0);
            MinHeight = 23;
            MinWidth = 75;
            IsCancel = true;
        }
    }
}
