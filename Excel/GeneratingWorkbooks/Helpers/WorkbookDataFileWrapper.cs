using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public class WorkbookDataFileWrapper
    {
        public enum enWorkbookDataFileHelperItemType
        {
            None = 0,
            CompetitionName = 1,
            MainJudge = 2,
            MainSecretary = 3,
            Row6 = 4
        }

        class FileItem
        {
            const string DELIMETER = "=+";

            public string Text = null;
            public enWorkbookDataFileHelperItemType Type = 0;

            public static FileItem Create(string text)
            {
                FileItem res = new FileItem();
                res.Text = text.Substring(0, text.IndexOf(DELIMETER)).Trim();
                int type;
                if (int.TryParse(text.Substring(text.IndexOf(DELIMETER) + DELIMETER.Length), out type))
                {
                    res.Type = (enWorkbookDataFileHelperItemType)type;
                }
                else
                    return null;

                return res;
            }

            public override string ToString()
            {
                return $"{Text} {DELIMETER}{(int)Type}";
            }
        }

        const string WORKBOOK_DATA_FILE_NAME = "data.txt";

        readonly string m_FilePath = null;
        readonly List<FileItem> m_FileItems = new List<FileItem>();


        public WorkbookDataFileWrapper(string dirFullPath)
        {
            try
            {
                m_FilePath = Path.Combine(dirFullPath, WORKBOOK_DATA_FILE_NAME);

                m_FileItems.Clear();
                foreach (var text in File.ReadAllLines(m_FilePath))
                {
                    var item = FileItem.Create(text);
                    if (item != null)
                        m_FileItems.Add(item);
                }
            }
            catch (Exception)
            {
                m_FileItems.Clear();
            }
        }

        public IList<string> GetStrings(enWorkbookDataFileHelperItemType type)
        {
            return m_FileItems.Where(arg => arg.Type == type).Select(arg => arg.Text).ToList();
        }

        public void AddItem(string text, enWorkbookDataFileHelperItemType type)
        {
            m_FileItems.Add(new FileItem()
            {
                Text = text,
                Type = type
            });
        }

        public bool Save()
        {
            try
            {
                File.WriteAllLines(m_FilePath, m_FileItems.Select(arg => arg.ToString()).ToArray());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
