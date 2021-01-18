using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace _3_In_Row
{
    class Field//Класс управляющий основными игровыми возможностями
    {
        const int height = 8, width = 10;
        public Unit[,] table = new Unit[height, width];
        int seed;
        public int combo = 1;
        public int points = 0;
        bool swapFlag = false;
        Unit clicedUnit = null, swapedUnit1, swapedUnit2;



        public Field()
        {
            //заполнение таблицы для поля
            //первый ряд сучайными, остальные пустыми 
            seed = (int)DateTime.Now.Ticks;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Unit u = new Unit(j, i, seed);
                    if (j != 0) { u.SetTipe(0); }
                    apdateSeed();
                    table[j, i] = u;
                    u.img.MouseDown += MouseDown;
                }
            }
        }

        void apdateSeed()
        {
            seed = seed * 7 + 11;//два простых числа для минимальной цикличности
        }

        public bool Drop()
        {
            //points = 0;
            bool ItsDroped = false;
            //проходим по таблице в обратном направлении
            for (int i = width - 1; i >= 0; i--)
            for (int j = height - 1; j >= 0; j--)
            if (table[j, i].tipe == 0)
            {//если ячейка пуста смещаем её тип и тип всех ячеек над ней на клутку вниз
                for (int k = j; k >= 0; k--)
                {
                    if (k == 0)//если это верхний ряд запоняем ячейку случайным типом
                    {
                        table[k, i].RandomTipe();
                        apdateSeed();                    
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
            for (int i = 0; i < width; i++)//проход по стобцам
            {
                int lastTipe = table[0, i].tipe, numberInLine = 1;//
                for (int j = 0; j < height; j++)
                {
                    if (lastTipe == table[j, i].tipe && j != 0) numberInLine++;
                    else
                    {
                        if (numberInLine > 2) combo++;
                        numberInLine = 1;
                    }
                    if (numberInLine > 2)
                    {
                        points += 10 * (combo );//добавение очков за линию
                        UnitsToDelete.Add(table[j - 2, i]);
                        UnitsToDelete.Add(table[j - 1, i]);
                        UnitsToDelete.Add(table[j, i]);                        
                    }
                    lastTipe = table[j, i].tipe;
                }
            }

            for (int j = 0; j < height; j++)//то же самое тоько по строкам
            {
                int lastTipe = table[j, 0].tipe, numberInLine = 1;
                for (int i = 1; i < width; i++)
                {
                    if (lastTipe == table[j, i].tipe && i != 0) numberInLine++;
                    else
                    {
                        if (numberInLine > 2) combo++;
                        numberInLine = 1;
                    }
                    if (numberInLine > 2)
                    {
                        points += 10 * (combo);
                        UnitsToDelete.Add(table[j, i - 2]);
                        UnitsToDelete.Add(table[j, i - 1]);
                        UnitsToDelete.Add(table[j, i]);
                    }
                    lastTipe = table[j, i].tipe;
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

        void tableOpacity (bool mode)
        {
            double opacity = 0.5 ;
            if (mode) opacity = 1;

            foreach (Unit unit in table)
            {
                unit.img.Opacity = opacity;
                unit.img.IsEnabled = mode;
            }
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)//логика хода
        {
            Image img = (Image)sender;
            //вычисление координат выбранной кнопки
            int x = (int)img.Tag / 100;
            int y = (int)img.Tag % 100;

            if (table[x, y] == clicedUnit)//отмена выбора при повторном клике
            {
                tableOpacity(true);
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
                    tableOpacity(true);
                    clicedUnit = null;
                }
                else
                {//логика выбора ячейки
                    tableOpacity(false);//блокировка всех ячеек
                   
                    {//разблокировка "креста" ячеек
                        table[x, y].img.Opacity = 1;
                        table[x, y].img.IsEnabled = true;
                        if (x != Field.height - 1)
                        {
                            table[x + 1, y].img.Opacity = 1;
                            table[x + 1, y].img.IsEnabled = true;
                        }
                        if (x!=0)
                        {
                            table[x - 1, y].img.Opacity = 1;
                            table[x - 1, y].img.IsEnabled = true;
                        }
                        if (y != Field.width - 1)
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
