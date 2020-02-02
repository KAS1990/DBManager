namespace DBManager
{
    /// <summary>
    /// Вариант стурктуры Point, в которой координаты - целые числа
    /// </summary>
    public struct PointI
    {
        private int m_X;
        private int m_Y;

        public int X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public int Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        public PointI(int X, int Y)
        {
            m_X = X;
            m_Y = Y;
        }


        //
        // Сводка:
        //     Сравнивает две структуры System.Windows.Point на предмет их неравенства.
        //
        // Параметры:
        //   point1:
        //     Первая точка для сравнения.
        //
        //   point2:
        //     Вторая точка для сравнения.
        //
        // Возвращает:
        //     true, если точки point1 и point2 имеют разные координаты PointI.X
        //     или PointI.Y; false, если точки point1 и point2 имеют одинаковые
        //     координаты PointI.X и PointI.Y.
        public static bool operator !=(PointI point1, PointI point2)
        {
            return !point1.Equals(point2);
        }


        //
        // Сводка:
        //     Сравнивает две структуры System.Windows.Point на предмет их равенства.
        //
        // Параметры:
        //   point1:
        //     Первая сравниваемая структура System.Windows.Point.
        //
        //   point2:
        //     Вторая сравниваемая структура System.Windows.Point.
        //
        // Возвращает:
        //     true, если обе координаты PointI.X и PointI.Y
        //     точек point1 и point2 равны; в противном случае — false.
        public static bool operator ==(PointI point1, PointI point2)
        {
            return point1.Equals(point2);
        }


        //
        // Сводка:
        //     Определяет, является ли указанный System.Object объектом System.Windows.Point
        //     и содержит ли он те же координаты, что и данный System.Windows.Point.
        //
        // Параметры:
        //   o:
        //     Объект System.Object для сравнения.
        //
        // Возвращает:
        //     true, если o является System.Windows.Point и содержит те же значения System.Windows.Point.X
        //     и System.Windows.Point.Y, что и данный System.Windows.Point; в противном
        //     случае — false.
        public override bool Equals(object o)
        {
            return o is PointI ? Equals((PointI)o) : false;
        }


        //
        // Сводка:
        //     Сравнивает две структуры System.Windows.Point на предмет их равенства.
        //
        // Параметры:
        //   value:
        //     Точка для сравнения с данным экземпляром.
        //
        // Возвращает:
        //     true, если обе структуры System.Windows.Point содержат одинаковые значения
        //     System.Windows.Point.X и System.Windows.Point.Y; в противном случае — false.
        public bool Equals(PointI value)
        {
            return value.X == X && value.Y == Y;
        }


        //
        // Сводка:
        //     Возвращает хэш-код данного экземпляра.
        //
        // Возвращает:
        //     32-разрядное целое число со знаком, являющееся хэш-кодом для данного экземпляра.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }


        public bool IsEmpty()
        {
            return Equals(new PointI(-1, -1));
        }
    }
}
