using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO.IsolatedStorage;

namespace _3_In_Row
{
    class RecordCore //логика обработки рекордов
    {
        public List<string> recordTable = new List<string>();
        const int n = 15;
        int[] ScoreList = new int[n + 1];
        string[] NameList = new string[n + 1];

        int score = 0;

        public RecordCore()
        {
            string line = "";
            //загрузка рекордов из файла (взято со stackoverflow)
            Assembly a = Assembly.GetExecutingAssembly();
            string[] str = a.GetManifestResourceNames();
            using (Stream stream = a.GetManifestResourceStream("_3_In_Row.Resources.Records.txt"))
            {
                StreamReader reader = new StreamReader(stream);
                for (int j = 0; j < 15; j++)
                {
                    line = reader.ReadLine();
                    recordTable.Add(line);
                }
            }
            //разбиение строк на массивы имен и рекордов
            int i = 0;
            foreach (string st in recordTable)
            {
                int mode = 0;
                foreach (char ch in st)
                {
                    if (ch == '|')
                    {
                        if (mode == 1) NameList[i] = line;
                        if (mode == 2) ScoreList[i] = int.Parse(line);
                        line = "";
                        mode++;
                    }
                    else
                        line += ch;
                }
                i++;

            }
        }
        public void addPoints(int points)//добавление очков
        {
            score += points;
        }

        public int getScore()//возврат счета
        {
            return score;
        }

        public void checkRecord(string name)//добавление счета и имени в массивы в нужное положения
        {
            ScoreList[n] = score;
            NameList[n] = " " + name;

            for (int i = n; i > 0; i--)
            {
                if (ScoreList[i] >= ScoreList[i - 1])
                {
                    int s = ScoreList[i];
                    string n = NameList[i];
                    ScoreList[i] = ScoreList[i - 1];
                    NameList[i] = NameList[i - 1];
                    ScoreList[i - 1] = s;
                    NameList[i - 1] = n;

                }
            }
            score = 0;
            updateTable();
        }

        void updateTable()//обновение листа рекордов
        {
            string outString = "";
            recordTable.Clear();
            for (int i = 0; i < 15; i++)//стандартизация длин имен
            {
                string name = NameList[i];
                while (name.Count() < 11) name += " ";
                string st = (i + 1).ToString("d2") + "|" + name + "|" + ScoreList[i].ToString("d7") + "|";
                recordTable.Add(st);
            }

            foreach (string line in recordTable)//преобразование иста в диную строку
            {
                outString += line + "\r\n";
            }

            //сохранение в файл (взято со stackoverflow)
            Assembly a = Assembly.GetExecutingAssembly();
            string[] str = a.GetManifestResourceNames();
            //string resourceFile;
            using (Stream stream = a.GetManifestResourceStream("_3_In_Row.Resources.Records.txt"))
            {

                string resourceFilePath = "..\\..\\Resources\\Records.txt";
                StreamWriter writer = new StreamWriter(resourceFilePath);
                writer.Write(outString);
                writer.Close();
            }

        }
    }
}
