using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace _3_In_Row
{
    class Field//Класс управляющий основными игровыми возможностями
    {
        const int x = 8, y = 10;
        public Unit[,] table = new Unit[x, y];
        int seed;
        public int combo = 1;
        public int points = 0;
        bool swapFlag = false;
        Unit clicedUnit = null, swapedUnit1, swapedUnit2;


        public Field()
        {
            //заполнение таблицы для поля
            //первый ряд сучайными, остальные пустыми 
            seed = 33 * (int)DateTime.Now.Ticks;
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    Unit u = new Unit(j, i, seed);
                    if (j != 0) { u.SetTipe(0); }
                    seed = seed * 7 + 11;
                    table[j, i] = u;
                    u.img.MouseDown += MouseDown;
                }
            }
        }

        public bool Drop()
        {
            //points = 0;
            bool ItsDroped = false;
            //проходим по таблице в обратном направлении
            for (int i = y - 1; i >= 0; i--)
            for (int j = x - 1; j >= 0; j--)
            if (table[j, i].tipe == 0)
            {//если ячейка пуста смещаем её тип и тип всех ячеек над ней на клутку вниз
                for (int k = j; k >= 0; k--)
                {
                    if (k == 0)//если это верхний ряд запоняем ячейку случайным типом
                    {
                        table[k, i].RandomTipe();
                        seed = seed * 7 + 11;
                    }
                    else table[k, i].SetTipe(table[k - 1, i].tipe);
                }
                ItsDroped = true;
                break;//если встретилась пустая ячейка в столбце, после смещения ппереходим к следующему 
            }
            return ItsDroped;//возвращаем удаось ли обустить
        }

        public bool CheckLiens() //проверка начичия трех и блоее в ряд, и удаление таковых
        {
            bool LineFound = false;
            points = 0;
            List<Unit> UnitsToDelete = new List<Unit>();//лист ячеек на удвление
            for (int i = 0; i < y; i++)//проход по стобцам, лучще лично обясню как это работает
            {
                int m = table[0, i].tipe, n = 1;
                for (int j = 0; j < x; j++)
                {
                    if (m == table[j, i].tipe && j != 0) n++;
                    else
                    {
                        if (n > 2) combo++;
                        n = 1;
                    }
                    if (n > 2)
                    {
                        points += 10 * (combo );//добавение очков за линию
                        UnitsToDelete.Add(table[j - 2, i]);
                        UnitsToDelete.Add(table[j - 1, i]);
                        UnitsToDelete.Add(table[j, i]);                        
                    }
                    m = table[j, i].tipe;
                }
            }

            for (int j = 0; j < x; j++)//то же самое тоько по строкам
            {
                int m = table[j, 0].tipe, n = 1;
                for (int i = 1; i < y; i++)
                {
                    if (m == table[j, i].tipe && i != 0) n++;
                    else
                    {
                        if (n > 2) combo++;
                        n = 1;
                    }
                    if (n > 2)
                    {
                        points += 10 * (combo);
                        UnitsToDelete.Add(table[j, i - 2]);
                        UnitsToDelete.Add(table[j, i - 1]);
                        UnitsToDelete.Add(table[j, i]);
                    }
                    m = table[j, i].tipe;
                }
            }



            foreach (Unit unit in UnitsToDelete) unit.SetTipe(0);
            if (UnitsToDelete.Count != 0) LineFound = true;
            else combo = 1;
            if (!LineFound && swapFlag)//проверка на удачность хода игрока
            {
                if (!(UnitsToDelete.Contains(swapedUnit1) || UnitsToDelete.Contains(swapedUnit2)))
                {
                    //если ход не образавал линии, поменять обратно
                    int tipe = swapedUnit1.tipe;
                    swapedUnit1.SetTipe(swapedUnit2.tipe);
                    swapedUnit2.SetTipe(tipe);
                } 
            }
            swapFlag = false;



            return LineFound;//вернуть были ли найдены 3 и более в ряд
        }
        public void MouseDown(object sender, MouseButtonEventArgs e)//логика хода
        {
            Image img = (Image)sender;
            //вычисление координат выбранной кнопки
            int x = int.Parse(img.Tag.ToString()) / 100;
            int y = int.Parse(img.Tag.ToString()) % 100;

            if (table[x, y] == clicedUnit)//отмена выбора при повторном клике
            {
                foreach (Unit unit in table)
                {
                    unit.img.Opacity = 1;
                    unit.img.IsEnabled = true;
                }
                clicedUnit = null;
            }
            else
            {
                if (clicedUnit != null)//обмен типами
                {
                    int tipe = clicedUnit.tipe;
                    clicedUnit.SetTipe(table[x, y].tipe);
                    table[x, y].SetTipe(tipe);
                    swapedUnit1 = table[x, y];
                    swapedUnit2 = clicedUnit;
                    swapFlag = true;
                    foreach (Unit unit in table)
                    {
                        unit.img.Opacity = 1;
                        unit.img.IsEnabled = true;
                    }
                    clicedUnit = null;
                }
                else
                {//логика выбора ячейки
                    foreach (Unit unit in table)//блокировка всех ячеек
                    {
                        unit.img.Opacity = 0.5;
                        unit.img.IsEnabled = false;
                    }
                    {//разблокировка "креста" ячеек
                        table[x, y].img.Opacity = 1;
                        table[x, y].img.IsEnabled = true;
                        if (x != Field.x-1)
                        {
                            table[x + 1, y].img.Opacity = 1;
                            table[x + 1, y].img.IsEnabled = true;
                        }
                        if (x!=0)
                        {
                            table[x - 1, y].img.Opacity = 1;
                            table[x - 1, y].img.IsEnabled = true;
                        }
                        if (y != Field.y-1)
                        {
                            table[x, y + 1].img.Opacity = 1;
                            table[x, y + 1].img.IsEnabled = true;
                        }
                        if (y!=0)
                        {
                            table[x, y - 1].img.Opacity = 1;
                            table[x, y - 1].img.IsEnabled = true;
                        }
                    }
                    clicedUnit = table[x, y];//запоминание выбраной ячейки
                }
            }
            
        }
    }
}
