using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
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
    /// Логика взаимодействия для OccupyAudience.xaml
    /// </summary>
    public partial class OccupyAudience : Window
    {
        public OccupyAudience()
        {
            InitializeComponent();
        }

        bool oneDel = false;
        private void changeAudienceInfo(object sender, DependencyPropertyChangedEventArgs e)
        {
            AudienceInfo.Text = $"Пара номер: {LessonInfo.lessonNumber}\nНомер аудитории: {LessonInfo.audienceNumber}\nСостояние: Свободен\n";
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                var ALlsubjects = dbContext.Subjects.ToList();
                var sortedSubjects = ALlsubjects.OrderBy(s => s.subject_name).ToList();

                comboBoxAllSubjects.ItemsSource = sortedSubjects.Select(s => s.subject_name).ToList();
                comboBoxAllSubjects.SelectedIndex = 0;
                var audiences = dbContext.Audiences.FirstOrDefault(a => a.id_audience == LessonInfo.audienceNumber);
                if (audiences.description == null)
                    AudienceInfo.Text += "Описание: Отсутствует";
                else
                    AudienceInfo.Text += $"Описание: {audiences.description}";
                if (!UserInfo.isAdmin)
                {
                    if (!UserInfo.isShowSubjects)
                    {
                        UserInfo.isShowSubjects = true;
                        var subjects = dbContext.Teachers.Where(t => t.id_teacher == UserInfo.teacher_id).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                        for (int i = 0; i < subjects.Count; i++)
                        {
                            comboBoxTeachersSubjects.Items.Add(subjects[i].ToString());
                        }
                        comboBoxTeachersSubjects.Items.Add("Другое");
                    }
                    comboBoxTeachersSubjects.SelectedIndex = 0;
                }
                else
                {
                    selectSubjectTB.Text = "Выберите преподавателя и предмет:";
                    comboBoxTeachersSubjects.Visibility = Visibility.Hidden;
                    comboBoxTeachers.Visibility = Visibility.Visible;
                    comboBoxAllSubjects.Visibility = Visibility.Visible;
                    if (!UserInfo.isShowSubjects)
                    {
                        var teachers = dbContext.Teachers.ToList();
                        for (int i = 0; i < teachers.Count; i++)
                        {
                            comboBoxTeachers.Items.Add(teachers[i].surname);
                        }
                        UserInfo.isShowSubjects = true;
                        if (!oneDel) { comboBoxTeachers.Items.RemoveAt(0); oneDel = true; }
                        comboBoxTeachers.SelectedIndex = 0;
                    }
                }
            }
        }

        private void selectTeachersSubjects(object sender, SelectionChangedEventArgs e) {
            if (comboBoxTeachersSubjects.SelectedItem != null)
            {
                if (comboBoxTeachersSubjects.SelectedItem == "Другое")
                {
                    selectSubjectTB.Text = "Выберите другой предмет:";
                    comboBoxAllSubjects.Visibility = Visibility.Visible;
                }
                else
                {
                    selectSubjectTB.Text = "Выберите предмет:";
                    comboBoxAllSubjects.Visibility = Visibility.Hidden;
                }
            }
        }


        //Кнопка закрытия
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Hide();
                WindowManager.mainForm.Show();
            }
            catch
            {
                Application.Current.Shutdown();
            }
        }


        private void occupyBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                var isTeacherInLesson = dbContext.Lessons.Where(t => t.id_teacher == UserInfo.teacher_id).ToList();
                //if (isTeacherInLesson.Count > 0) MessageBox.Show("На эту пару за вами уже закреплена\nаудитория! Освободите её чтобы занять\nдругую.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                //else {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите занять\nданную аудиторию?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        string subjectSelected;
                        if (comboBoxTeachersSubjects.SelectedItem != "Другое" && !UserInfo.isAdmin) subjectSelected = comboBoxTeachersSubjects.SelectedItem.ToString();
                        else subjectSelected = comboBoxAllSubjects.SelectedItem.ToString();

                        // Находим запись в таблице Lessons по lesson_number и id_audience
                        var lesson = dbContext.Lessons.FirstOrDefault(l => l.lesson_number == LessonInfo.lessonNumber && l.id_audience == LessonInfo.audienceNumber);

                        if (lesson != null)
                        {
                            // Обновляем данные в найденной записи
                            if (UserInfo.isAdmin)
                            {
                                string selectTeacher = comboBoxTeachers.SelectedItem.ToString();
                                Teachers t = dbContext.Teachers.FirstOrDefault(l => l.surname == selectTeacher);
                                lesson.id_teacher = t.id_teacher;
                            }
                            else lesson.id_teacher = (byte)UserInfo.teacher_id;
                            var sub = dbContext.Subjects.FirstOrDefault(s => s.subject_name == subjectSelected);
                            lesson.id_subject = sub.id_subject;

                            // Сохраняем изменения
                            dbContext.SaveChanges();
                            MessageBox.Show("Аудитория была успешно Занята!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                            Hide();
                            WindowManager.mainForm.Show();
                        }
                   // }
                }
            }
        }
    }
}
