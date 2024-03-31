using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AudienceBusynessApp
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        public MainForm()
        {
            InitializeComponent();
            getAudience();
            comboBoxMohinacii();
            notFoundText.Text = "Ничего не было найдено \nпо выбранным критериям";

            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                var aud = dbContext.Audiences.ToList();
                for (int i = 0; i < aud.Count; i++)
                {
                    comboBoxAllAudiences.Items.Add(aud[i].id_audience);
                }
                var teachers = dbContext.Teachers.ToList();
                for (int i = 0; i < teachers.Count; i++)
                {
                    comboBoxTeachers.Items.Add(teachers[i].surname);
                }
                comboBoxTeachers.Items.RemoveAt(1);
                comboBoxTeachers.SelectedIndex = 0;
            }
        }

        void comboBoxMohinacii()
        {
            comboBoxAllAudiences.Items.Add("Любая");
            comboBoxAllAudiences.SelectedIndex = 0;
            comboBoxAllLessonNum.Items.Add("Неважно");
            comboBoxAllLessonNum.Items.Add("1");
            comboBoxAllLessonNum.Items.Add("2");
            comboBoxAllLessonNum.Items.Add("3");
            comboBoxAllLessonNum.Items.Add("4");
            comboBoxAllLessonNum.Items.Add("5");
            comboBoxAllLessonNum.Items.Add("6");
            comboBoxTeachers.Items.Add("Все преподаватели");
            comboBoxBusy.Items.Add("Любая");
            comboBoxBusy.Items.Add("Свободна");
            comboBoxBusy.Items.Add("Занята");
            comboBoxBusy.SelectedIndex = 0;
            comboBoxAllLessonNum.SelectedIndex = 0;
            comboBoxTeachers.SelectedIndex = 0;
        }

        //Каждый раз когда показываем эту форма данные о занятости аудиторий обновлялись
        private void changeLessonsSchedule(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (UserInfo.login == null)
            {
                profileBtn.Visibility = Visibility.Hidden;
            }
            getAudience();
            if (!UserInfo.isAdmin) openAdminPanel.Visibility = Visibility.Hidden;
            byte[] imageData;
            using (var db = new CollegeAuditoriumsEntities())
            {
                var teacher = db.Teachers.FirstOrDefault(t => t.id_teacher == UserInfo.teacher_id);
                if (teacher != null)
                {
                    imageData = teacher.profile_image;

                    // Преобразование массива байтов в BitmapImage    
                    if (imageData != null)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            stream.Position = 0;
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }

                        // Создание и установка ImageBrush
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = bitmap;

                        // Установка ImageBrush в Fill свойство Ellipse
                        profileBtn.Fill = imageBrush;
                    }
                }
            }
        }


        //Заполняет listItem аудиториями и инфу о них
        void getStringForTextBox(TextBlock tb, int lessonNum, int audienceNum)
        {
            using (var BD = new CollegeAuditoriumsEntities())
            {
                Lessons ls = BD.Lessons.FirstOrDefault(u => u.id_audience == audienceNum && u.lesson_number == lessonNum);
                tb.Text = $"Аудитория №{ls.id_audience}\n";
                tb.FontSize = 13;
                tb.FontWeight = FontWeights.SemiBold;
                tb.LineHeight = 12;
                if (ls.id_teacher != 1)
                {

                    Teachers teacher = BD.Teachers.FirstOrDefault(t => t.id_teacher == ls.id_teacher);
                    tb.Text += $"{teacher.surname}\nЗанят";
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x75, 0x75));
                }
                else
                {
                    tb.Text += $"Свободна";
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
                }
            }
        }

        private void openAdminPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
            WindowManager.adminPanelForm.Show();
        }



        int lessonNow = 1;
        //Кнопка переключения на пару вперед
        private void arrowBtnRight_Click(object sender, RoutedEventArgs e)
        {
            toTheRight();
        }
        //Кнопка переключения на пару назад
        private void arrowBtnLeft_Click(object sender, RoutedEventArgs e)
        {
            toTheLeft();
        }
        //Переключение пар через стрелочки
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                toTheLeft();
            }
            else if (e.Key == Key.Right)
            {
                toTheRight();
            }
        }

        void toTheLeft()
        {
            if (lessonNow == 2)
            {
                lessonNow--;
                lessonNumberLabel.Content = "Пара 1";
                lessonTimeLabel.Content = "8:30 - 9:50";
                lesson1.Visibility = Visibility.Visible;
                lesson2.Visibility = Visibility.Hidden;
            }
            else if (lessonNow == 3)
            {
                lessonNow--;
                lessonNumberLabel.Content = "Пара 2";
                lessonTimeLabel.Content = "10:00 - 11:20";
                lesson2.Visibility = Visibility.Visible;
                lesson3.Visibility = Visibility.Hidden;
            }
            else if (lessonNow == 4)
            {
                lessonNow--;
                lessonNumberLabel.Content = "Пара 3";
                lessonTimeLabel.Content = "11:40 - 13:00";
                lesson3.Visibility = Visibility.Visible;
                lesson4.Visibility = Visibility.Hidden;
            }
            else if (lessonNow == 5)
            {
                lessonNow--;
                lessonNumberLabel.Content = "Пара 4";
                lessonTimeLabel.Content = "13:20 - 14:40";
                lesson4.Visibility = Visibility.Visible;
                lesson5.Visibility = Visibility.Hidden;
            }
            else if (lessonNow == 6)
            {
                lessonNow--;
                lessonNumberLabel.Content = "Пара 5";
                lessonTimeLabel.Content = "15:00 - 16:20";
                lesson5.Visibility = Visibility.Visible;
                lesson6.Visibility = Visibility.Hidden;
            }
        }
        void toTheRight()
        {
            if (lessonNow == 1)
            {
                lessonNow++;
                lessonNumberLabel.Content = "Пара 2";
                lessonTimeLabel.Content = "10:00 - 11:20";
                lesson1.Visibility = Visibility.Hidden;
                lesson2.Visibility = Visibility.Visible;
            }
            else if (lessonNow == 2)
            {
                lessonNow++;
                lessonNumberLabel.Content = "Пара 3";
                lessonTimeLabel.Content = "11:40 - 13:00";
                lesson2.Visibility = Visibility.Hidden;
                lesson3.Visibility = Visibility.Visible;
            }
            else if (lessonNow == 3)
            {
                lessonNow++;
                lessonNumberLabel.Content = "Пара 4";
                lessonTimeLabel.Content = "13:20 - 14:40";
                lesson3.Visibility = Visibility.Hidden;
                lesson4.Visibility = Visibility.Visible;
            }
            else if (lessonNow == 4)
            {
                lessonNow++;
                lessonNumberLabel.Content = "Пара 5";
                lessonTimeLabel.Content = "15:00 - 16:20";
                lesson4.Visibility = Visibility.Hidden;
                lesson5.Visibility = Visibility.Visible;
            }
            else if (lessonNow == 5)
            {
                lessonNow++;
                lessonNumberLabel.Content = "Пара 6";
                lessonTimeLabel.Content = "16:30 - 17:50";
                lesson5.Visibility = Visibility.Hidden;
                lesson6.Visibility = Visibility.Visible;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Application.Current.Shutdown();
        }

        private void profileBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            WindowManager.userProfile.Show();
        }
        private void exitProfBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        bool isSearch = false;
        private void searcAndClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isSearch)
            {
                isSearch = true;
                BitmapImage newImage = new BitmapImage(new Uri("/close-search.png", UriKind.Relative));
                searcAndClose.Source = newImage;
                ToolTip toolTip = (ToolTip)((Image)sender).ToolTip;
                toolTip.Content = "Закрыть поиск";
                SheduleText.Content = "Поиск аудиторий";
                lessonNumberLabel.Content = "по фильтрам";
                lessonTimeLabel.Content = "";
                searchBlock.Visibility = Visibility.Visible;
                lesson1.Visibility = Visibility.Hidden;
                lesson2.Visibility = Visibility.Hidden;
                lesson3.Visibility = Visibility.Hidden;
                lesson4.Visibility = Visibility.Hidden;
                lesson5.Visibility = Visibility.Hidden;
                lesson6.Visibility = Visibility.Hidden;

                arrowBtnRight.IsEnabled = false;
                arrowBtnLeft.IsEnabled = false;
            }
            else
            {
                isSearch = false;

                SheduleText.Content = "Список аудиторий";
                BitmapImage newImage = new BitmapImage(new Uri("/search.png", UriKind.Relative));
                searcAndClose.Source = newImage;
                ToolTip toolTip = (ToolTip)((Image)sender).ToolTip;
                toolTip.Content = "Поиск аудиторий";
                searchBlock.Visibility = Visibility.Hidden;
                switch(lessonNow)
                {
                    case 1: lesson1.Visibility = Visibility.Visible;
                        lessonNumberLabel.Content = "Пара 1";
                        lessonTimeLabel.Content = "8:30 - 9:50";
                        break;
                    case 2: lesson2.Visibility = Visibility.Visible;
                        lessonNumberLabel.Content = "Пара 2";
                        lessonTimeLabel.Content = "10:00 - 11:20"; 
                        break;
                    case 3: lesson3.Visibility = Visibility.Visible;
                        lessonNumberLabel.Content = "Пара 3";
                        lessonTimeLabel.Content = "11:40 - 13:00";
                        break;
                    case 4: lesson4.Visibility = Visibility.Visible;
                        lessonNumberLabel.Content = "Пара 4";
                        lessonTimeLabel.Content = "13:20 - 14:40";
                        break;
                    case 5: lesson5.Visibility = Visibility.Visible;
                        lessonNumberLabel.Content = "Пара 5";
                        lessonTimeLabel.Content = "15:00 - 16:20"; 
                        break;
                    case 6: lesson6.Visibility = Visibility.Visible;
                        lessonNumberLabel.Content = "Пара 6";
                        lessonTimeLabel.Content = "16:30 - 17:50"; 
                        break;
                }

                arrowBtnRight.IsEnabled = true;
                arrowBtnLeft.IsEnabled = true;
            }
        }


        private void clean_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            searchAudiencesListBox.Items.Clear();
            comboBoxAllAudiences.SelectedIndex = 0;
            comboBoxBusy.SelectedIndex = 0;
            comboBoxAllLessonNum.SelectedIndex = 0;
            comboBoxTeachers.SelectedIndex = 0;
            notFoundBlock.Visibility = Visibility.Hidden;
        }

        private void search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            searchAudiencesListBox.Items.Clear();

            notFoundBlock.Visibility = Visibility.Hidden;
            searchAudiencesListBox.Visibility = Visibility.Visible;
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                if(comboBoxAllLessonNum.SelectedItem == "Неважно" && comboBoxTeachers.SelectedItem == "Все преподаватели" && comboBoxBusy.SelectedItem == "Любая" && comboBoxAllAudiences.SelectedItem == "Любая")
                {
                    MessageBox.Show("Выберите хотя бы один фильтр! Иначе смысла использования фильтра нет!", "Предупреждение!", MessageBoxButton.OK,
                MessageBoxImage.Warning);
                }
                else
                {

                    var query = dbContext.Lessons.AsQueryable();

                    if (comboBoxAllAudiences.SelectedItem != "Любая")
                    {
                        int audNum = int.Parse(comboBoxAllAudiences.SelectedItem.ToString());
                        query = query.Where(l => l.id_audience == audNum);
                    }

                    if (comboBoxAllLessonNum.SelectedItem != "Неважно")
                    {
                        int allLeson = int.Parse(comboBoxAllLessonNum.SelectedItem.ToString());
                        query = query.Where(l => l.lesson_number == allLeson);
                    }

                    if (comboBoxTeachers.SelectedItem != "Все преподаватели")
                    {
                        string teacherName = comboBoxTeachers.SelectedItem.ToString();
                        query = query.Where(l => l.Teachers.surname == teacherName);
                    }

                    if (comboBoxBusy.SelectedItem == "Свободна")
                    {
                        query = query.Where(l => l.id_teacher == 1);
                    }

                    if (comboBoxBusy.SelectedItem == "Занята")
                    {
                        query = query.Where(l => l.id_teacher != 1);
                    }



                    var filteredLessons = query.ToList();

                    if(filteredLessons.Count == 0)
                    {
                        notFoundBlock.Visibility = Visibility.Visible;
                        searchAudiencesListBox.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        audSerachFun(filteredLessons);
                    }
                }
                searchAudiencesListBox.MouseDoubleClick += searchAudiencesListBox_MouseDoubleClick;
            }
        }

        void audSerachFun(List<Lessons> audiences)
        {
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                for (int i = 0; i < audiences.Count; i++)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = audiences[i].id_audience.ToString();
                    textBlock.FontSize = 13;
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.FontWeight = FontWeights.SemiBold;
                    textBlock.Name = "textBlock";
                    TextBlock textBlock2 = new TextBlock();
                    textBlock2.Text = audiences[i].id_audience.ToString();
                    textBlock2.FontSize = 14;
                    textBlock2.TextAlignment = TextAlignment.Center;
                    textBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock2.FontWeight = FontWeights.SemiBold;
                    textBlock2.Name = "textBlock2";

                    var teacherId = (int)audiences[i].id_teacher;
                    var teacher = dbContext.Teachers.FirstOrDefault(t => t.id_teacher == teacherId);
                    textBlock.Text = $"Аудитория №{audiences[i].id_audience}\n";
                    textBlock2.Text = "Пара №" + audiences[i].lesson_number.ToString();

                    TextBlock textBlock3 = new TextBlock();

                    textBlock3.Text = $"{audiences[i].lesson_number}|{audiences[i].id_audience}|";
                    textBlock3.Name = "infoForClick";
                    textBlock3.Visibility = Visibility.Hidden;

                    if (audiences[i].id_teacher != 1)
                    {
                        textBlock.Text += $"{teacher.surname}\nЗанят";
                        textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x75, 0x75));
                    }
                    else
                    {
                        textBlock.Text += $"Свободна";
                        textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
                    }

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Children.Add(textBlock2);
                    stackPanel.Children.Add(textBlock);

                    stackPanel.Children.Add(textBlock3);
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanel.VerticalAlignment = VerticalAlignment.Center;

                    // Создайте ListBoxItem и добавьте StackPanel в него
                    ListBoxItem item = new ListBoxItem();
                    item.Content = stackPanel;
                    // Добавьте ListBoxItem в ListBox
                    item.HorizontalContentAlignment = HorizontalAlignment.Center;
                    item.VerticalContentAlignment = VerticalAlignment.Center;
                    searchAudiencesListBox.Items.Add(item);
                }
            }
        }
       

        private void searchAudiencesListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            ListBoxItem selectedItem = listBox.SelectedItem as ListBoxItem;
            TextBlock textBlock = FindChild<TextBlock>(selectedItem, "textBlock");
            TextBlock textBox3 = FindChild<TextBlock>(selectedItem, "infoForClick");
            string textFromTextBox1 = textBlock.Text;
            string textFromTextBox3 = textBox3.Text;

            string[] numbers = textFromTextBox3.Split('|');
            int lessonNum = int.Parse(numbers[0]);
            int audNum = int.Parse(numbers[1]);

            // Далее можно использовать содержимое текстовых полей для выполнения нужных действий
            freeUpOrOccupyAudienceDo(textBlock, lessonNum, audNum);
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childType && ((FrameworkElement)child).Name == childName)
                {
                    return childType;
                }

                T result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }

            return null;
        }

        //Запускает окно освободить или занять аудиторию в зависимости от цвета текста
        //также сохраняет в статический класс номер аудитории и номер пары чтобы получить данные с БД
        void freeUpOrOccupyAudienceDo(TextBlock tb, int lessonNum, int audienceNum)
        {
            if (UserInfo.login != null)
            {
                using (var BD = new CollegeAuditoriumsEntities())
                {
                    Lessons lesson = BD.Lessons.FirstOrDefault(u => u.id_audience == audienceNum && u.lesson_number == lessonNum);
                    LessonInfo.lessonNumber = lesson.lesson_number;
                    LessonInfo.audienceNumber = lesson.id_audience;
                    LessonInfo.idTeacher = lesson.id_teacher;
                    var s = BD.Subjects.FirstOrDefault(t => t.id_subject == lesson.id_subject);
                    if (s != null) LessonInfo.subject = s.subject_name;
                    else LessonInfo.subject = "";
                }
                if (((SolidColorBrush)tb.Foreground).Color == Color.FromRgb(0x4C, 0xAF, 0x50)) { Hide(); WindowManager.occupyAudience.Show(); }
                else { Hide(); WindowManager.freeUpAudience.Show(); }
            }
            else MessageBox.Show("Ваш уровень доступа 'Студент', вы не можете занимать или освобождать аудитории!", "Ошибка!", MessageBoxButton.OK,
                 MessageBoxImage.Error);
        }

        //Ниже АД
        //Ниже для каждой кнопки в listbox создается обработчик события со своими параментрами
        //Пара номер 1
        private void aud11Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud11, 1, 11);
        }
        private void aud12Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud12, 1, 12);
        }
        private void aud13Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud13, 1, 13);
        }
        private void aud14Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud14, 1, 14);
        }
        private void aud15Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud15, 1, 15);
        }
        private void aud16Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud16, 1, 16);
        }
        private void aud17Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud17, 1, 17);
        }
        private void aud18Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud18, 1, 18);
        }
        private void aud21Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud21, 1, 21);
        }
        private void aud22Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud22, 1, 22);
        }
        private void aud23Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud23, 1, 23);
        }
        private void aud24Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud24, 1, 24);
        }
        private void aud31Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud31, 1, 31);
        }
        private void aud32Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud32, 1, 32);
        }
        private void aud33Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud33, 1, 33);
        }
        private void aud34Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud34, 1, 34);
        }
        private void aud35Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud35, 1, 35);
        }
        private void aud36Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud36, 1, 36);
        }
        private void aud41Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud41, 1, 41);
        }
        private void aud42Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud42, 1, 42);
        }
        private void aud43Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud43, 1, 43);
        }
        private void aud44Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud44, 1, 44);
        }
        private void aud45Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud45, 1, 45);
        }
        private void aud46Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud46, 1, 46);
        }
        //Пара номер 2
        private void aud112Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud112, 2, 11);
        }
        private void aud122Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud122, 2, 12);
        }
        private void aud132Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud132, 2, 13);
        }
        private void aud142Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud142, 2, 14);
        }
        private void aud152Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud152, 2, 15);
        }
        private void aud162Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud162, 2, 16);
        }
        private void aud172Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud172, 2, 17);
        }
        private void aud182Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud182, 2, 18);
        }
        private void aud212Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud212, 2, 21);
        }
        private void aud222Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud222, 2, 22);
        }
        private void aud232Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud232, 2, 23);
        }
        private void aud242Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud242, 2, 24);
        }
        private void aud312Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud312, 2, 31);
        }
        private void aud322Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud322, 2, 32);
        }
        private void aud332Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud332, 2, 33);
        }
        private void aud342Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud342, 2, 34);
        }
        private void aud352Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud352, 2, 35);
        }
        private void aud362Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud362, 2, 36);
        }
        private void aud412Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud412, 2, 41);
        }
        private void aud422Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud422, 2, 42);
        }
        private void aud432Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud432, 2, 43);
        }
        private void aud442Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud442, 2, 44);
        }
        private void aud452Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud452, 2, 45);
        }
        private void aud462Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud462, 2, 46);
        }
        //Пара номер 3
        private void aud113Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud113, 3, 11);
        }
        private void aud123Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud123, 3, 12);
        }
        private void aud133Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud133, 3, 13);
        }
        private void aud143Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud143, 3, 14);
        }
        private void aud153Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud153, 3, 15);
        }
        private void aud163Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud163, 3, 16);
        }
        private void aud173Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud173, 3, 17);
        }
        private void aud183Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud183, 3, 18);
        }
        private void aud213Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud213, 3, 21);
        }
        private void aud223Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud223, 3, 22);
        }
        private void aud233Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud233, 3, 23);
        }
        private void aud243Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud243, 3, 24);
        }
        private void aud313Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud313, 3, 31);
        }
        private void aud323Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud323, 3, 32);
        }
        private void aud333Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud333, 3, 33);
        }
        private void aud343Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud343, 3, 34);
        }
        private void aud353Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud353, 3, 35);
        }
        private void aud363Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud363, 3, 36);
        }
        private void aud413Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud413, 3, 41);
        }
        private void aud423Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud423, 3, 42);
        }
        private void aud433Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud433, 3, 43);
        }
        private void aud443Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud443, 3, 44);
        }
        private void aud453Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud453, 3, 45);
        }
        private void aud463Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud463, 3, 46);
        }
        //Пара номер 4
        private void aud114Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud114, 4, 11);
        }
        private void aud124Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud124, 4, 12);
        }
        private void aud134Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud134, 4, 13);
        }
        private void aud144Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud144, 4, 14);
        }
        private void aud154Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud154, 4, 15);
        }
        private void aud164Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud164, 4, 16);
        }
        private void aud174Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud174, 4, 17);
        }
        private void aud184Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud184, 4, 18);
        }
        private void aud214Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud214, 4, 21);
        }
        private void aud224Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud224, 4, 22);
        }
        private void aud234Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud234, 4, 23);
        }
        private void aud244Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud244, 4, 24);
        }
        private void aud314Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud314, 4, 31);
        }
        private void aud324Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud324, 4, 32);
        }
        private void aud334Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud334, 4, 33);
        }
        private void aud344Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud344, 4, 34);
        }
        private void aud354Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud354, 4, 35);
        }
        private void aud364Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud364, 4, 36);
        }
        private void aud414Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud414, 4, 41);
        }
        private void aud424Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud424, 4, 42);
        }
        private void aud434Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud434, 4, 43);
        }
        private void aud444Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud444, 4, 44);
        }
        private void aud454Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud454, 4, 45);
        }
        private void aud464Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud464, 4, 46);
        }
        //Пара номер 5
        private void aud115Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud115, 5, 11);
        }
        private void aud125Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud125, 5, 12);
        }
        private void aud135Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud135, 5, 13);
        }
        private void aud145Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud145, 5, 14);
        }
        private void aud155Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud155, 5, 15);
        }
        private void aud165Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud165, 5, 16);
        }
        private void aud175Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud175, 5, 17);
        }
        private void aud185Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud185, 5, 18);
        }
        private void aud215Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud215, 5, 21);
        }
        private void aud225Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud225, 5, 22);
        }
        private void aud235Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud235, 5, 23);
        }
        private void aud245Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud245, 5, 24);
        }
        private void aud315Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud315, 5, 31);
        }
        private void aud325Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud325, 5, 32);
        }
        private void aud335Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud335, 5, 33);
        }
        private void aud345Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud345, 5, 34);
        }
        private void aud355Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud355, 5, 35);
        }
        private void aud365Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud365, 5, 36);
        }
        private void aud415Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud415, 5, 41);
        }
        private void aud425Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud425, 5, 42);
        }
        private void aud435Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud435, 5, 43);
        }
        private void aud445Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud445, 5, 44);
        }
        private void aud455Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud455, 5, 45);
        }
        private void aud465Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud465, 5, 46);
        }
        //Пара номер 6
        private void aud116Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud116, 6, 11);
        }
        private void aud126Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud126, 6, 12);
        }
        private void aud136Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud136, 6, 13);
        }
        private void aud146Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud146, 6, 14);
        }
        private void aud156Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud156, 6, 15);
        }
        private void aud166Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud166, 6, 16);
        }
        private void aud176Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud176, 6, 17);
        }
        private void aud186Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud186, 6, 18);
        }
        private void aud216Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud216, 6, 21);
        }
        private void aud226Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud226, 6, 22);
        }
        private void aud236Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud236, 6, 23);
        }
        private void aud246Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud246, 6, 24);
        }
        private void aud316Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud316, 6, 31);
        }
        private void aud326Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud326, 6, 32);
        }
        private void aud336Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud336, 6, 33);
        }
        private void aud346Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud346, 6, 34);
        }
        private void aud356Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud356, 6, 35);
        }
        private void aud366Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud366, 6, 36);
        }
        private void aud416Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud416, 6, 41);
        }
        private void aud426Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud426, 6, 42);
        }
        private void aud436Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud436, 6, 43);
        }
        private void aud446Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud446, 6, 44);
        }
        private void aud456Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud456, 6, 45);
        }
        private void aud466Box_DoubleClick(object sender, RoutedEventArgs e)
        {
            freeUpOrOccupyAudienceDo(aud466, 6, 46);
        }

        //Эта функция просто куча раз вызывает другую функцию
        void getAudience()
        {
            getStringForTextBox(aud11, 1, 11);
            getStringForTextBox(aud12, 1, 12);
            getStringForTextBox(aud13, 1, 13);
            getStringForTextBox(aud14, 1, 14);
            getStringForTextBox(aud15, 1, 15);
            getStringForTextBox(aud16, 1, 16);
            getStringForTextBox(aud17, 1, 17);
            getStringForTextBox(aud18, 1, 18);
            getStringForTextBox(aud21, 1, 21);
            getStringForTextBox(aud22, 1, 22);
            getStringForTextBox(aud23, 1, 23);
            getStringForTextBox(aud24, 1, 24);
            getStringForTextBox(aud31, 1, 31);
            getStringForTextBox(aud32, 1, 32);
            getStringForTextBox(aud33, 1, 33);
            getStringForTextBox(aud34, 1, 34);
            getStringForTextBox(aud35, 1, 35);
            getStringForTextBox(aud36, 1, 36);
            getStringForTextBox(aud41, 1, 41);
            getStringForTextBox(aud42, 1, 42);
            getStringForTextBox(aud43, 1, 43);
            getStringForTextBox(aud44, 1, 44);
            getStringForTextBox(aud45, 1, 45);
            getStringForTextBox(aud46, 1, 46);
            getStringForTextBox(aud112, 2, 11);
            getStringForTextBox(aud122, 2, 12);
            getStringForTextBox(aud132, 2, 13);
            getStringForTextBox(aud142, 2, 14);
            getStringForTextBox(aud152, 2, 15);
            getStringForTextBox(aud162, 2, 16);
            getStringForTextBox(aud172, 2, 17);
            getStringForTextBox(aud182, 2, 18);
            getStringForTextBox(aud212, 2, 21);
            getStringForTextBox(aud222, 2, 22);
            getStringForTextBox(aud232, 2, 23);
            getStringForTextBox(aud242, 2, 24);
            getStringForTextBox(aud312, 2, 31);
            getStringForTextBox(aud322, 2, 32);
            getStringForTextBox(aud332, 2, 33);
            getStringForTextBox(aud342, 2, 34);
            getStringForTextBox(aud352, 2, 35);
            getStringForTextBox(aud362, 2, 36);
            getStringForTextBox(aud412, 2, 41);
            getStringForTextBox(aud422, 2, 42);
            getStringForTextBox(aud432, 2, 43);
            getStringForTextBox(aud442, 2, 44);
            getStringForTextBox(aud452, 2, 45);
            getStringForTextBox(aud462, 2, 46);
            getStringForTextBox(aud113, 3, 11);
            getStringForTextBox(aud123, 3, 12);
            getStringForTextBox(aud133, 3, 13);
            getStringForTextBox(aud143, 3, 14);
            getStringForTextBox(aud153, 3, 15);
            getStringForTextBox(aud163, 3, 16);
            getStringForTextBox(aud173, 3, 17);
            getStringForTextBox(aud183, 3, 18);
            getStringForTextBox(aud213, 3, 21);
            getStringForTextBox(aud223, 3, 22);
            getStringForTextBox(aud233, 3, 23);
            getStringForTextBox(aud243, 3, 24);
            getStringForTextBox(aud313, 3, 31);
            getStringForTextBox(aud323, 3, 32);
            getStringForTextBox(aud333, 3, 33);
            getStringForTextBox(aud343, 3, 34);
            getStringForTextBox(aud353, 3, 35);
            getStringForTextBox(aud363, 3, 36);
            getStringForTextBox(aud413, 3, 41);
            getStringForTextBox(aud423, 3, 42);
            getStringForTextBox(aud433, 3, 43);
            getStringForTextBox(aud443, 3, 44);
            getStringForTextBox(aud453, 3, 45);
            getStringForTextBox(aud463, 3, 46);

            getStringForTextBox(aud114, 4, 11);
            getStringForTextBox(aud124, 4, 12);
            getStringForTextBox(aud134, 4, 13);
            getStringForTextBox(aud144, 4, 14);
            getStringForTextBox(aud154, 4, 15);
            getStringForTextBox(aud164, 4, 16);
            getStringForTextBox(aud174, 4, 17);
            getStringForTextBox(aud184, 4, 18);
            getStringForTextBox(aud214, 4, 21);
            getStringForTextBox(aud224, 4, 22);
            getStringForTextBox(aud234, 4, 23);
            getStringForTextBox(aud244, 4, 24);
            getStringForTextBox(aud314, 4, 31);
            getStringForTextBox(aud324, 4, 32);
            getStringForTextBox(aud334, 4, 33);
            getStringForTextBox(aud344, 4, 34);
            getStringForTextBox(aud354, 4, 35);
            getStringForTextBox(aud364, 4, 36);
            getStringForTextBox(aud414, 4, 41);
            getStringForTextBox(aud424, 4, 42);
            getStringForTextBox(aud434, 4, 43);
            getStringForTextBox(aud444, 4, 44);
            getStringForTextBox(aud454, 4, 45);
            getStringForTextBox(aud464, 4, 46);

            getStringForTextBox(aud115, 5, 11);
            getStringForTextBox(aud125, 5, 12);
            getStringForTextBox(aud135, 5, 13);
            getStringForTextBox(aud145, 5, 14);
            getStringForTextBox(aud155, 5, 15);
            getStringForTextBox(aud165, 5, 16);
            getStringForTextBox(aud175, 5, 17);
            getStringForTextBox(aud185, 5, 18);
            getStringForTextBox(aud215, 5, 21);
            getStringForTextBox(aud225, 5, 22);
            getStringForTextBox(aud235, 5, 23);
            getStringForTextBox(aud245, 5, 24);
            getStringForTextBox(aud315, 5, 31);
            getStringForTextBox(aud325, 5, 32);
            getStringForTextBox(aud335, 5, 33);
            getStringForTextBox(aud345, 5, 34);
            getStringForTextBox(aud355, 5, 35);
            getStringForTextBox(aud365, 5, 36);
            getStringForTextBox(aud415, 5, 41);
            getStringForTextBox(aud425, 5, 42);
            getStringForTextBox(aud435, 5, 43);
            getStringForTextBox(aud445, 5, 44);
            getStringForTextBox(aud455, 5, 45);
            getStringForTextBox(aud465, 5, 46);

            getStringForTextBox(aud116, 6, 11);
            getStringForTextBox(aud126, 6, 12);
            getStringForTextBox(aud136, 6, 13);
            getStringForTextBox(aud146, 6, 14);
            getStringForTextBox(aud156, 6, 15);
            getStringForTextBox(aud166, 6, 16);
            getStringForTextBox(aud176, 6, 17);
            getStringForTextBox(aud186, 6, 18);
            getStringForTextBox(aud216, 6, 21);
            getStringForTextBox(aud226, 6, 22);
            getStringForTextBox(aud236, 6, 23);
            getStringForTextBox(aud246, 6, 24);
            getStringForTextBox(aud316, 6, 31);
            getStringForTextBox(aud326, 6, 32);
            getStringForTextBox(aud336, 6, 33);
            getStringForTextBox(aud346, 6, 34);
            getStringForTextBox(aud356, 6, 35);
            getStringForTextBox(aud366, 6, 36);
            getStringForTextBox(aud416, 6, 41);
            getStringForTextBox(aud426, 6, 42);
            getStringForTextBox(aud436, 6, 43);
            getStringForTextBox(aud446, 6, 44);
            getStringForTextBox(aud456, 6, 45);
            getStringForTextBox(aud466, 6, 46);
        }
        //Изменяет размер текста при изменении экрана
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidth = this.ActualWidth; // Получаем текущую ширину окна
            if (windowWidth > 1500)
            {
                aud11.FontSize = 18;
                aud12.FontSize = 18;
                aud13.FontSize = 18;
                aud14.FontSize = 18;
                aud15.FontSize = 18;
                aud16.FontSize = 18;
                aud17.FontSize = 18;
                aud18.FontSize = 18;
                aud21.FontSize = 18;
                aud22.FontSize = 18;
                aud23.FontSize = 18;
                aud24.FontSize = 18;
                aud31.FontSize = 18;
                aud32.FontSize = 18;
                aud33.FontSize = 18;
                aud34.FontSize = 18;
                aud35.FontSize = 18;
                aud36.FontSize = 18;
                aud41.FontSize = 18;
                aud42.FontSize = 18;
                aud43.FontSize = 18;
                aud44.FontSize = 18;
                aud45.FontSize = 18;
                aud46.FontSize = 18;

                aud112.FontSize = 18;
                aud122.FontSize = 18;
                aud132.FontSize = 18;
                aud142.FontSize = 18;
                aud152.FontSize = 18;
                aud162.FontSize = 18;
                aud172.FontSize = 18;
                aud182.FontSize = 18;
                aud212.FontSize = 18;
                aud222.FontSize = 18;
                aud232.FontSize = 18;
                aud242.FontSize = 18;
                aud312.FontSize = 18;
                aud322.FontSize = 18;
                aud332.FontSize = 18;
                aud342.FontSize = 18;
                aud352.FontSize = 18;
                aud362.FontSize = 18;
                aud412.FontSize = 18;
                aud422.FontSize = 18;
                aud432.FontSize = 18;
                aud442.FontSize = 18;
                aud452.FontSize = 18;
                aud462.FontSize = 18;

                aud113.FontSize = 18;
                aud123.FontSize = 18;
                aud133.FontSize = 18;
                aud143.FontSize = 18;
                aud153.FontSize = 18;
                aud163.FontSize = 18;
                aud173.FontSize = 18;
                aud183.FontSize = 18;
                aud213.FontSize = 18;
                aud223.FontSize = 18;
                aud233.FontSize = 18;
                aud243.FontSize = 18;
                aud313.FontSize = 18;
                aud323.FontSize = 18;
                aud333.FontSize = 18;
                aud343.FontSize = 18;
                aud353.FontSize = 18;
                aud363.FontSize = 18;
                aud413.FontSize = 18;
                aud423.FontSize = 18;
                aud433.FontSize = 18;
                aud443.FontSize = 18;
                aud453.FontSize = 18;
                aud463.FontSize = 18;

                aud114.FontSize = 18;
                aud124.FontSize = 18;
                aud134.FontSize = 18;
                aud144.FontSize = 18;
                aud154.FontSize = 18;
                aud164.FontSize = 18;
                aud174.FontSize = 18;
                aud184.FontSize = 18;
                aud214.FontSize = 18;
                aud224.FontSize = 18;
                aud234.FontSize = 18;
                aud244.FontSize = 18;
                aud314.FontSize = 18;
                aud324.FontSize = 18;
                aud334.FontSize = 18;
                aud344.FontSize = 18;
                aud354.FontSize = 18;
                aud364.FontSize = 18;
                aud414.FontSize = 18;
                aud424.FontSize = 18;
                aud434.FontSize = 18;
                aud444.FontSize = 18;
                aud454.FontSize = 18;
                aud464.FontSize = 18;

                aud115.FontSize = 18;
                aud125.FontSize = 18;
                aud135.FontSize = 18;
                aud145.FontSize = 18;
                aud155.FontSize = 18;
                aud165.FontSize = 18;
                aud175.FontSize = 18;
                aud185.FontSize = 18;
                aud215.FontSize = 18;
                aud225.FontSize = 18;
                aud235.FontSize = 18;
                aud245.FontSize = 18;
                aud315.FontSize = 18;
                aud325.FontSize = 18;
                aud335.FontSize = 18;
                aud345.FontSize = 18;
                aud355.FontSize = 18;
                aud365.FontSize = 18;
                aud415.FontSize = 18;
                aud425.FontSize = 18;
                aud435.FontSize = 18;
                aud445.FontSize = 18;
                aud455.FontSize = 18;
                aud465.FontSize = 18;

                aud116.FontSize = 18;
                aud126.FontSize = 18;
                aud136.FontSize = 18;
                aud146.FontSize = 18;
                aud156.FontSize = 18;
                aud166.FontSize = 18;
                aud176.FontSize = 18;
                aud186.FontSize = 18;
                aud216.FontSize = 18;
                aud226.FontSize = 18;
                aud236.FontSize = 18;
                aud246.FontSize = 18;
                aud316.FontSize = 18;
                aud326.FontSize = 18;
                aud336.FontSize = 18;
                aud346.FontSize = 18;
                aud356.FontSize = 18;
                aud366.FontSize = 18;
                aud416.FontSize = 18;
                aud426.FontSize = 18;
                aud436.FontSize = 18;
                aud446.FontSize = 18;
                aud456.FontSize = 18;
                aud466.FontSize = 18;
            }
            else
            {
                aud11.FontSize = 13;
                aud12.FontSize = 13;
                aud13.FontSize = 13;
                aud14.FontSize = 13;
                aud15.FontSize = 13;
                aud16.FontSize = 13;
                aud17.FontSize = 13;
                aud18.FontSize = 13;
                aud21.FontSize = 13;
                aud22.FontSize = 13;
                aud23.FontSize = 13;
                aud24.FontSize = 13;
                aud31.FontSize = 13;
                aud32.FontSize = 13;
                aud33.FontSize = 13;
                aud34.FontSize = 13;
                aud35.FontSize = 13;
                aud36.FontSize = 13;
                aud41.FontSize = 13;
                aud42.FontSize = 13;
                aud43.FontSize = 13;
                aud44.FontSize = 13;
                aud45.FontSize = 13;
                aud46.FontSize = 13;

                aud112.FontSize = 13;
                aud122.FontSize = 13;
                aud132.FontSize = 13;
                aud142.FontSize = 13;
                aud152.FontSize = 13;
                aud162.FontSize = 13;
                aud172.FontSize = 13;
                aud182.FontSize = 13;
                aud212.FontSize = 13;
                aud222.FontSize = 13;
                aud232.FontSize = 13;
                aud242.FontSize = 13;
                aud312.FontSize = 13;
                aud322.FontSize = 13;
                aud332.FontSize = 13;
                aud342.FontSize = 13;
                aud352.FontSize = 13;
                aud362.FontSize = 13;
                aud412.FontSize = 13;
                aud422.FontSize = 13;
                aud432.FontSize = 13;
                aud442.FontSize = 13;
                aud452.FontSize = 13;
                aud462.FontSize = 13;
                aud113.FontSize = 13;
                aud123.FontSize = 13;
                aud133.FontSize = 13;
                aud143.FontSize = 13;
                aud153.FontSize = 13;
                aud163.FontSize = 13;
                aud173.FontSize = 13;
                aud183.FontSize = 13;
                aud213.FontSize = 13;
                aud223.FontSize = 13;
                aud233.FontSize = 13;
                aud243.FontSize = 13;
                aud313.FontSize = 13;
                aud323.FontSize = 13;
                aud333.FontSize = 13;
                aud343.FontSize = 13;
                aud353.FontSize = 13;
                aud363.FontSize = 13;
                aud413.FontSize = 13;
                aud423.FontSize = 13;
                aud433.FontSize = 13;
                aud443.FontSize = 13;
                aud453.FontSize = 13;
                aud463.FontSize = 13;
                aud114.FontSize = 13;
                aud124.FontSize = 13;
                aud134.FontSize = 13;
                aud144.FontSize = 13;
                aud154.FontSize = 13;
                aud164.FontSize = 13;
                aud174.FontSize = 13;
                aud184.FontSize = 13;
                aud214.FontSize = 13;
                aud224.FontSize = 13;
                aud234.FontSize = 13;
                aud244.FontSize = 13;
                aud314.FontSize = 13;
                aud324.FontSize = 13;
                aud334.FontSize = 13;
                aud344.FontSize = 13;
                aud354.FontSize = 13;
                aud364.FontSize = 13;
                aud414.FontSize = 13;
                aud424.FontSize = 13;
                aud434.FontSize = 13;
                aud444.FontSize = 13;
                aud454.FontSize = 13;
                aud464.FontSize = 13;
                aud115.FontSize = 13;
                aud125.FontSize = 13;
                aud135.FontSize = 13;
                aud145.FontSize = 13;
                aud155.FontSize = 13;
                aud165.FontSize = 13;
                aud175.FontSize = 13;
                aud185.FontSize = 13;
                aud215.FontSize = 13;
                aud225.FontSize = 13;
                aud235.FontSize = 13;
                aud245.FontSize = 13;
                aud315.FontSize = 13;
                aud325.FontSize = 13;
                aud335.FontSize = 13;
                aud345.FontSize = 13;
                aud355.FontSize = 13;
                aud365.FontSize = 13;
                aud415.FontSize = 13;
                aud425.FontSize = 13;
                aud435.FontSize = 13;
                aud445.FontSize = 13;
                aud455.FontSize = 13;
                aud465.FontSize = 13;
                aud116.FontSize = 13;
                aud126.FontSize = 13;
                aud136.FontSize = 13;
                aud146.FontSize = 13;
                aud156.FontSize = 13;
                aud166.FontSize = 13;
                aud176.FontSize = 13;
                aud186.FontSize = 13;
                aud216.FontSize = 13;
                aud226.FontSize = 13;
                aud236.FontSize = 13;
                aud246.FontSize = 13;
                aud316.FontSize = 13;
                aud326.FontSize = 13;
                aud336.FontSize = 13;
                aud346.FontSize = 13;
                aud356.FontSize = 13;
                aud366.FontSize = 13;
                aud416.FontSize = 13;
                aud426.FontSize = 13;
                aud436.FontSize = 13;
                aud446.FontSize = 13;
                aud456.FontSize = 13;
                aud466.FontSize = 13;
            }
        }

        private void aud11Box_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
