using DBManager.Global;

namespace DBManager
{
    public partial class members
    {
        /// <summary>
        /// Равны ли данные об участниках без учёта индексов
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool OnlyDataFieldsEqual(members rhs)
        {
            return name == rhs.name &&
                    surname == rhs.surname &&
                    year_of_birth == rhs.year_of_birth &&
                    sex == rhs.sex;
        }

        public string ToInfoString()
        {
            return $"{surname.ToUpper(0)} {name.ToUpper(0)} {year_of_birth}";
        }
    }
}
