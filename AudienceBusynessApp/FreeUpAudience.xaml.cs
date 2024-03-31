using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AudienceBusynessApp
{
    /// <summary>
    /// Логика взаимодействия для FreeUpAudience.xaml
    /// </summary>
    public partial class FreeUpAudience : Window
    {
        public FreeUpAudience()
        {
            InitializeComponent();
        }

        private void changeAudienceInfo(object sender, DependencyPropertyChangedEventArgs e)
        {
           
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                Teachers teacher;
                AudienceInfo.Text = $"Пара номер: {LessonInfo.lessonNumber}\nНомер аудитории: {LessonInfo.audienceNumber}";
                teacher = dbContext.Teachers.FirstOrDefault(l => l.id_teacher == LessonInfo.idTeacher);
                AudienceInfo.Text += $"\nПреподаватель: {teacher.surname}\nПредмет: {LessonInfo.subject}\nСостояние: Занят\n";
                var audiences = dbContext.Audiences.FirstOrDefault(a => a.id_audience == LessonInfo.audienceNumber);
                if (audiences.description == null)
                    AudienceInfo.Text += "Описание: Отсутствует";
                else
                    AudienceInfo.Text += $"Описание: {audiences.description}";
            }
        }

        void freeUpAudience(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите освободить\n данную аудиторию?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var dbContext = new CollegeAuditoriumsEntities())
                {
                    // Находим запись в таблице Lessons по lesson_number и id_audience
                    var lesson = dbContext.Lessons.FirstOrDefault(l => l.lesson_number == LessonInfo.lessonNumber && l.id_audience == LessonInfo.audienceNumber);

                    if (lesson != null)
                    {
                        // Обновляем данные в найденной записи
                        lesson.id_teacher = 1;
                        lesson.id_subject = null;

                        // Сохраняем изменения
                        dbContext.SaveChanges();
                        MessageBox.Show("Аудитория была успешно освобождена!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                        Hide();
                        WindowManager.mainForm.Show();
                    }
                }
            } 
        }

        //Кнопка справа наверху закрытия окна
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Hide();
                WindowManager.mainForm.Show();
            }
            catch { 
                Application.Current.Shutdown();
            }
        }
    }
}
