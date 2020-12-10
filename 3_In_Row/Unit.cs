using _3_In_Row.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace _3_In_Row
{
    class Unit//класс логики ячейки
    {
       
        public int X, Y, tipe;
        public Image img = new Image();
        Random rand;


        public Unit(int x, int y, int seed)
        {
            rand = new Random(seed+x+y);
            img.Tag = x * 100 + y;
            X = x;
            Y = y;
            RandomTipe();           
        }

 
        public void RandomTipe()//установка рандомного типа
        {
            tipe = (rand.Next() % 5 + 1);
            SetTipe(tipe);            
        }
        public void SetTipe(int t)//установка точного типа
        {
            tipe = t;
            BitmapImage imgS = new BitmapImage(new Uri("pack://application:,,,/Resources/" + tipe + ".png"));
            img.Source = imgS;
        } 
    }
}
