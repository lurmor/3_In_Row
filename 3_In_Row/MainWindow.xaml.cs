using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3_In_Row
{
    public partial class MainWindow : Window
    {
        Field filed = new Field();
        System.Windows.Threading.DispatcherTimer Timer;
        System.Windows.Threading.DispatcherTimer MainTimer;
        RecordCore recordCore = new RecordCore();
        int timeLeft = 0;
        string name="";
        public MainWindow()
        {
            InitializeComponent();
            Timer = new System.Windows.Threading.DispatcherTimer();
            Timer.Tick += new EventHandler(tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            MainTimer = new System.Windows.Threading.DispatcherTimer();
            MainTimer.Tick += new EventHandler(tack);
            MainTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);

            BitmapImage imgS = new BitmapImage(new Uri("pack://application:,,,/Resources/Cell.png"));

            //Создание фона поля игры
            foreach (Unit unit in filed.table)
            {
                Image Cell = new Image();
                Cell.Source = imgS;
                Cell.Opacity = 0.7;
                BGG.Children.Add(Cell);
            }
            
            RecordList.ItemsSource = recordCore.recordTable;
        }

        void tack(object sender, EventArgs e)
        {//обратный отсчет
            if (timeLeft == 0)
            {
                stopGame();
            }
            else
            {
                timeLeft -= 1;
                TimeL.Content = timeLeft;
            }
        }

        void stopGame()
        {//остановка игры
            MainTimer.Stop();
            Timer.Stop();
            UFG.Children.Clear();
            PlayBT.Visibility = Visibility.Visible;
            NameBox.Visibility = Visibility.Visible;
            recordCore.checkRecord(name);
            RecordList.ItemsSource = null /*recordCore.recordTable*/;
            RecordList.ItemsSource = recordCore.recordTable;
        }

        void tick(object sender, EventArgs e) //"шаг"игры 
        {
            
            if (!filed.Drop()) //пытаемся опустить шарики
                if (filed.CheckLiens())//если невозможно, пытаемся удалить линии
                    recordCore.addPoints(filed.points);//если хоть одна линия удалилась добавляем подсчитаные очки
            if (filed.combo > 1) ComboL.Content = "COMBO X" + (filed.combo );
            else ComboL.Content = "";
            string st = recordCore.getScore().ToString("d7");
            SourseL.Content = st;
        }

        private void PlayBT_MouseDown(object sender, MouseButtonEventArgs e)//старт игры
        {
            if(NameBox.Text!="")
            {
                name = NameBox.Text;
                NameBox.Visibility = Visibility.Hidden;
                PlayBT.Visibility = Visibility.Hidden;
                filed = new Field();
                BitmapImage imgS = new BitmapImage(new Uri("pack://application:,,,/Resources/Cell.png"));
                timeLeft = 60;
                MainTimer.Start();

                foreach (Unit unit in filed.table)
                {
                    UFG.Children.Add(unit.img);
                }
                Timer.Start();
            }
        }
    }
}
