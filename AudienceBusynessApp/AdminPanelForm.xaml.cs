using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using System.Xml.Linq;

namespace AudienceBusynessApp
{
    /// <summary>
    /// Логика взаимодействия для AdminPanelForm.xaml
    /// </summary>
    public partial class AdminPanelForm : Window
    {
        public AdminPanelForm()
        {
            InitializeComponent();
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
            catch
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        bool isShow = false;
        private void changeAudienceInfo(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!isShow)
            {
                isShow = true;
                using (var dbContext = new CollegeAuditoriumsEntities())
                {
                    var teachers = dbContext.Teachers.ToList();
                    for (int i = 0; i < teachers.Count; i++)
                    {
                        comboBoxTeachers.Items.Add(teachers[i].surname);
                        comboBoxTeachersForSubjects.Items.Add(teachers[i].surname);
                    }
                    comboBoxTeachers.Items.RemoveAt(0);
                    comboBoxTeachersForSubjects.Items.RemoveAt(0);
                }
            }
            addNewTheacher.Visibility = Visibility.Collapsed;
            teacherAndSubjects.Visibility = Visibility.Collapsed;
            editTeachers.Visibility = Visibility.Collapsed;
            editSubjectBlock.Visibility = Visibility.Collapsed;
            addSubjectBlock.Visibility = Visibility.Collapsed;
            editTeacherBtn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            addTeacherBtn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            editTecherAndSubject.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            editSubjects.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            addSubjects.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
        }

        int idTeacher;
        private void comboBoxTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboBoxTeachers.SelectedIndex != -1)
            {
                selectedIndex = comboBoxTeachers.SelectedIndex;
                byte[] imageData;
                using (var dbContext = new CollegeAuditoriumsEntities())
                {
                    var teacher = dbContext.Teachers.FirstOrDefault(t => t.surname == comboBoxTeachers.SelectedItem);
                    login.Text = teacher.login;
                    pasword.Text = teacher.password;
                    surname.Text = teacher.surname;
                    name.Text = teacher.name;
                    lastname.Text = teacher.lastname;
                    idTeacher = teacher.id_teacher;
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
                        profileImg.Fill = imageBrush;
                    }
                    else
                    {
                        ImageSource defaultImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                        ImageBrush defaultImageBrush = new ImageBrush();
                        defaultImageBrush.ImageSource = defaultImageSource;
                        profileImg.Fill = defaultImageBrush;
                    }


                    editTeacherInfo.Visibility = Visibility.Visible;
                }
            }
        }

        byte[] imageData;
        void editProfileImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // Получение выбранного пути к изображению
                string selectedImagePath = openFileDialog.FileName;

                // Загрузка выбранного изображения в Ellipse
                BitmapImage bitmap = new BitmapImage(new Uri(selectedImagePath));
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = bitmap;

                profileImg.Fill = imageBrush;

                using (MemoryStream stream = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                    imageData = stream.ToArray();
                }
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollegeAuditoriumsEntities())
            {
                Teachers teacher = db.Teachers.FirstOrDefault(t => t.id_teacher == idTeacher); // Предполагая, что у вашей модели Teacher есть соответствующий столбец id
                if (teacher != null)
                {
                    if(imageData != null) teacher.profile_image = imageData;
                    teacher.login = login.Text;
                    teacher.surname = surname.Text;
                    teacher.name = name.Text;
                    teacher.lastname = lastname.Text;
                    teacher.password = pasword.Text;
                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    comboBoxTeachers.Items.Clear();
                    using (var dbContext = new CollegeAuditoriumsEntities())
                    {
                        var teachers = dbContext.Teachers.ToList();
                        for (int i = 0; i < teachers.Count; i++)
                        {
                            comboBoxTeachers.Items.Add(teachers[i].surname);
                        }
                        comboBoxTeachers.Items.RemoveAt(0);
                    }
                    comboBoxTeachers.SelectedIndex = selectedIndex;
                }
            }
        }

        int selectedIndex = 0;
        private void deleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить\nпреподавателя из базы данных?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new CollegeAuditoriumsEntities())
                {
                    var teacher = db.Teachers.Include(t => t.Subjects).FirstOrDefault(t => t.id_teacher == idTeacher); ;
                    var lessonTeacher = db.Lessons.Where(l => l.id_teacher == teacher.id_teacher).ToList();
                    foreach (var lesson in lessonTeacher)
                    {
                        lesson.id_teacher = 1;
                        lesson.id_subject = null;
                    }
                    teacher.Subjects.Clear();
                    db.Teachers.Remove(teacher);
                    db.SaveChanges();
                    MessageBox.Show("Преподаватель был успешно удален!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    comboBoxTeachers.Items.Clear();
                    using (var dbContext = new CollegeAuditoriumsEntities())
                    {
                        var teachers = dbContext.Teachers.ToList();
                        for (int i = 0; i < teachers.Count; i++)
                        {
                            comboBoxTeachers.Items.Add(teachers[i].surname);
                        }
                        comboBoxTeachers.Items.RemoveAt(0);
                    }
                    comboBoxTeachers.SelectedIndex = -1;
                    editTeacherInfo.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Меняет фон кнопки делая ее активной
            changeBackgroundBtn(sender as Button);

            editTeachers.Visibility = Visibility.Visible;
            teacherAndSubjects.Visibility = Visibility.Collapsed;
            addNewTheacher.Visibility = Visibility.Collapsed;
            editSubjectBlock.Visibility = Visibility.Collapsed;
            addSubjectBlock.Visibility = Visibility.Collapsed;
            comboBoxTeachers.Items.Clear();
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                var teachers = dbContext.Teachers.ToList();
                for (int i = 0; i < teachers.Count; i++)
                {
                    comboBoxTeachers.Items.Add(teachers[i].surname);
                }
                comboBoxTeachers.Items.RemoveAt(0);
            }
            comboBoxTeachers.SelectedIndex = -1;
            editTeacherInfo.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Меняет фон кнопки делая ее активной
            changeBackgroundBtn(sender as Button);

            editTeacherAndSubject.Visibility = Visibility.Collapsed;
            addNewTheacher.Visibility = Visibility.Collapsed;
            editSubjectBlock.Visibility = Visibility.Collapsed;
            addSubjectBlock.Visibility = Visibility.Collapsed;
            comboBoxTeachersForSubjects.Items.Clear();
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                var teachers = dbContext.Teachers.ToList();
                for (int i = 0; i < teachers.Count; i++)
                {
                    comboBoxTeachersForSubjects.Items.Add(teachers[i].surname);
                }
                comboBoxTeachersForSubjects.Items.RemoveAt(0);
            }
            comboBoxTeachersForSubjects.SelectedIndex = -1;
            teacherAndSubjects.Visibility = Visibility.Visible;
            editTeachers.Visibility = Visibility.Collapsed;
        }

        private void comboBoxTeachersForSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxTeachersForSubjects.SelectedIndex != -1)
            {
                selectedIndex = comboBoxTeachers.SelectedIndex;
                using (var dbContext = new CollegeAuditoriumsEntities())
                {
                    var teacher = dbContext.Teachers.FirstOrDefault(t => t.surname == comboBoxTeachersForSubjects.SelectedItem);
                    fullNameSelectedTeacher.Text = teacher.surname + " " + teacher.name + " " + teacher.lastname;

                    editTeacherInfo.Visibility = Visibility.Visible;

                    var subjects = dbContext.Teachers.Where(t => t.id_teacher == teacher.id_teacher).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                    for (int i = 0; i < subjects.Count; i++)
                    {
                        subjects[i] = $"{i + 1}. {subjects[i]}";
                    }
                    subjectsItemsControl.ItemsSource = subjects;
                    idTeacher = teacher.id_teacher;
                   

                    var Allsubjects = dbContext.Subjects.ToList();
                    var sortedSubjects = Allsubjects.OrderBy(s => s.subject_name).ToList();

                    comboBoxAllSubjects.ItemsSource = sortedSubjects.Select(s => s.subject_name).ToList();
                    comboBoxAllSubjects.SelectedIndex = 0;
                    comboBoxTeachersSubjects.Items.Clear();
                    var subjects2 = dbContext.Teachers.Where(t => t.id_teacher == idTeacher).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                    for (int i = 0; i < subjects2.Count; i++)
                    {
                        comboBoxTeachersSubjects.Items.Add(subjects2[i].ToString());
                    }
                }
                comboBoxTeachersSubjects.SelectedIndex = 0;
                editTeacherAndSubject.Visibility = Visibility.Visible;
            }
        }

        private void addSubject_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollegeAuditoriumsEntities())
            {
                //Ищем препода по id
                var teacher = db.Teachers.FirstOrDefault(t => t.id_teacher == idTeacher);
                //Запихиваем выбранный предмет в comboBox в переменную для удобства
                var subjectName = comboBoxAllSubjects.SelectedItem.ToString();
                // Поиск предмета по названию
                var subject = db.Subjects.FirstOrDefault(s => s.subject_name == subjectName);
                bool alreadyAssigned = teacher.Subjects.Any(s => s.subject_name == subject.subject_name);
                if (!alreadyAssigned)
                {
                    // Добавление предмета к преподавателю
                    teacher.Subjects.Add(subject);
                    db.SaveChanges();

                    MessageBox.Show("Предмет успешно был закреплен к преподавателю!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                    var subjects = db.Teachers.Where(t => t.id_teacher == teacher.id_teacher).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                    for (int i = 0; i < subjects.Count; i++)
                    {
                        subjects[i] = $"{i + 1}. {subjects[i]}";
                    }

                    idTeacher = teacher.id_teacher;
                    subjectsItemsControl.ItemsSource = subjects;
                    var subjects2 = db.Teachers.Where(t => t.id_teacher == idTeacher).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                    comboBoxTeachersSubjects.Items.Clear();
                    for (int i = 0; i < subjects2.Count; i++)
                    {
                        comboBoxTeachersSubjects.Items.Add(subjects2[i].ToString());
                    }
                    comboBoxTeachersSubjects.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Предмет уже закреплен за этим преподавателем!","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        private void deleteSubject_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollegeAuditoriumsEntities())
            {
                var teacher = db.Teachers.Include(t => t.Subjects).FirstOrDefault(t => t.id_teacher == idTeacher);
                if (teacher != null)
                {
                    var subjectToRemove = teacher.Subjects.FirstOrDefault(s => s.subject_name == comboBoxTeachersSubjects.SelectedItem.ToString());

                    if (subjectToRemove != null)
                    {
                        teacher.Subjects.Remove(subjectToRemove);
                        db.SaveChanges();
                        MessageBox.Show("Предмет успешно откреплен от преподавателя!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                        var subjects = db.Teachers.Where(t => t.id_teacher == teacher.id_teacher).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                        for (int i = 0; i < subjects.Count; i++)
                        {
                            subjects[i] = $"{i + 1}. {subjects[i]}";
                        }

                        idTeacher = teacher.id_teacher;
                        subjectsItemsControl.ItemsSource = subjects;
                        var subjects2 = db.Teachers.Where(t => t.id_teacher == idTeacher).SelectMany(t => t.Subjects).Select(s => s.subject_name).ToList();
                        comboBoxTeachersSubjects.Items.Clear();
                        for (int i = 0; i < subjects2.Count; i++)
                        {
                            comboBoxTeachersSubjects.Items.Add(subjects2[i].ToString());
                        }
                        comboBoxTeachersSubjects.SelectedIndex = 0;
                    }
                }
            }
        }

        void changeBackgroundBtn(Button button)
        {
            editTeacherBtn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            addTeacherBtn.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            editTecherAndSubject.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            editSubjects.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));
            addSubjects.Background = new SolidColorBrush(Color.FromArgb(255, 255, 254, 241));

            button.Background = new SolidColorBrush(Color.FromArgb(255, 255, 243, 84));
        }

        private void addTeacherBtn_Click(object sender, RoutedEventArgs e)
        {
            //Меняет фон кнопки делая ее активной
            changeBackgroundBtn(sender as Button);
            addNewTheacher.Visibility = Visibility.Visible;
            teacherAndSubjects.Visibility = Visibility.Collapsed;
            editTeachers.Visibility = Visibility.Collapsed;
            editSubjectBlock.Visibility = Visibility.Collapsed;
            addSubjectBlock.Visibility = Visibility.Collapsed;
        }

        private void editSubjects_Click(object sender, RoutedEventArgs e)
        {
            //Меняет фон кнопки делая ее активной
            changeBackgroundBtn(sender as Button);
            editTeachers.Visibility = Visibility.Collapsed;
            teacherAndSubjects.Visibility = Visibility.Collapsed;
            addNewTheacher.Visibility = Visibility.Collapsed;
            editSubjectBlock.Visibility = Visibility.Visible;
            addSubjectBlock.Visibility = Visibility.Collapsed;
            comboBoxSubjects.Items.Clear();
            using (var dbContext = new CollegeAuditoriumsEntities())
            {
                var subjects = dbContext.Subjects.ToList();
                for (int i = 0; i < subjects.Count; i++)
                {
                    comboBoxSubjects.Items.Add(subjects[i].subject_name);
                }
            }
            comboBoxSubjects.SelectedIndex = -1;
            editSubjectsGrid.Visibility = Visibility.Hidden;
        }

        private void addSubjects_Click(object sender, RoutedEventArgs e)
        {
            //Меняет фон кнопки делая ее активной
            changeBackgroundBtn(sender as Button);
            editTeachers.Visibility = Visibility.Collapsed;
            teacherAndSubjects.Visibility = Visibility.Collapsed;
            addNewTheacher.Visibility = Visibility.Collapsed;
            editSubjectBlock.Visibility = Visibility.Collapsed;
            addSubjectBlock.Visibility = Visibility.Visible;
            descSubj.Text = "";
            newSubjectTextBox.Text = "";
        }

        private void addNewTeacherBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollegeAuditoriumsEntities())
            {
                if(login2.Text != "" || pasword2.Text != "" || surname2.Text != "" || name2.Text != "")
                {
                    if(login2.Text.Length > 4)
                    {
                        if (pasword2.Text.Length > 3)
                        {
                            if (surname2.Text.Length > 3 || name2.Text.Length > 3)
                            {
                                int maxTeacherId = db.Teachers.Max(t => t.id_teacher);
                                Teachers newTeacher = new Teachers();
                                newTeacher.id_teacher = (byte)(maxTeacherId + 1);
                                newTeacher.login = login2.Text;
                                newTeacher.password = pasword2.Text;
                                newTeacher.surname = surname2.Text;
                                newTeacher.name = name2.Text;
                                if (lastname2.Text != "") newTeacher.lastname = lastname2.Text;
                                if (imageData != null) newTeacher.profile_image = imageData;
                                db.Teachers.Add(newTeacher);
                                db.SaveChanges();
                                MessageBox.Show("Новый преподаватель был успешно добавлен!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                                login2.Text = "";
                                pasword2.Text = "";
                                name2.Text = "";
                                surname2.Text = "";
                                lastname2.Text = "";
                                imageData2 = null;
                                ImageSource defaultImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                                ImageBrush defaultImageBrush = new ImageBrush();
                                defaultImageBrush.ImageSource = defaultImageSource;
                                profileImg2.Fill = defaultImageBrush;
                            }
                            else MessageBox.Show("Фамилия и имя должны состоят минимум\nиз трех символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else MessageBox.Show("Пароль должен состоят минимум\nиз четырех символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else MessageBox.Show("Логин должен состоят минимум\nиз пяти символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show("Вы ввели не все данные!","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        byte[] imageData2;
        void editProfileImage2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // Получение выбранного пути к изображению
                string selectedImagePath = openFileDialog.FileName;

                // Загрузка выбранного изображения в Ellipse
                BitmapImage bitmap = new BitmapImage(new Uri(selectedImagePath));
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = bitmap;

                profileImg2.Fill = imageBrush;

                using (MemoryStream stream = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                    imageData2 = stream.ToArray();
                }
            }
        }

        private void addNewSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollegeAuditoriumsEntities())
            {
                if (newSubjectTextBox.Text != "")
                {
                    if (newSubjectTextBox.Text.Length > 2)
                    {
                        var sub = db.Subjects.FirstOrDefault(s => s.subject_name == newSubjectTextBox.Text);
                        if(sub == null)
                        {
                            Subjects subjects = new Subjects();
                            subjects.subject_name = newSubjectTextBox.Text;
                            if(descSubj.Text != "") subjects.subject_desc = descSubj.Text;
                            int countSub = db.Subjects.Max(t => t.id_subject);
                            subjects.id_subject = countSub + 1;
                            db.Subjects.Add(subjects);
                            db.Subjects.Add(subjects);
                            db.SaveChanges(); 
                            MessageBox.Show("Новый предмет был успешно добавлен!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                            descSubj.Text = "";
                            newSubjectTextBox.Text = "";
                        }
                        else MessageBox.Show("Такой предмет уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);                        
                    }
                    else MessageBox.Show("Название предмета должно состоять\nминимум из трех символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show("Вы не ввели название нового предмета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        string sub;
        private void comboBoxSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxSubjects.SelectedIndex != -1)
            {
                selectedIndexSubject = comboBoxSubjects.SelectedIndex;
                editSubjectsGrid.Visibility = Visibility.Visible;
                using (var dbContext = new CollegeAuditoriumsEntities())
                {
                    var subject = dbContext.Subjects.FirstOrDefault(t => t.subject_name == comboBoxSubjects.SelectedItem);
                    sub = subject.subject_name;
                    subjectsNameTextBox.Text = subject.subject_name;
                    if (subject.subject_desc != null) edtDescSubj.Text = subject.subject_desc;
                    else edtDescSubj.Text = "Описание отсутствует.";
                }
            }  
        }

        private void editSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CollegeAuditoriumsEntities())
            {
                var subject = db.Subjects.FirstOrDefault(s => s.subject_name == sub);
                subject.subject_name = subjectsNameTextBox.Text;
                if(edtDescSubj.Text != "" && edtDescSubj.Text != "Описание отсутствует.") subject.subject_desc = edtDescSubj.Text;
                else subject.subject_desc = null;
                db.SaveChanges();
                MessageBox.Show("Предмет был успешно изменен!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                subjectsNameTextBox.Text = "";
                edtDescSubj.Text = "";
                comboBoxSubjects.Items.Clear();
                // Обновление комбобокса с преподавателями после удаления
                comboBoxSubjects.Items.Clear();
                using (var dbContext = new CollegeAuditoriumsEntities())
                {
                    var subjects = dbContext.Subjects.ToList();
                    foreach (var sub in subjects)
                    {
                        comboBoxSubjects.Items.Add(sub.subject_name);
                    }
                    comboBoxSubjects.SelectedIndex = selectedIndexSubject;
                }
            }
        }

        int selectedIndexSubject = 0;
        private void deleteSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить\nпредмет из базы данных?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new CollegeAuditoriumsEntities())
                {
                    var subject = db.Subjects.Include(s => s.Teachers).FirstOrDefault(s => s.subject_name == sub);

                    if (subject != null)
                    {
                        foreach (var teacher in subject.Teachers.ToList())
                        {
                            teacher.Subjects.Remove(subject);
                        }

                        db.Subjects.Remove(subject);
                        db.SaveChanges();

                        MessageBox.Show("Предмет был успешно удален!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                        subjectsNameTextBox.Text = "";
                        edtDescSubj.Text = "";

                        var lessonTeacher = db.Lessons.Where(l => l.id_subject == subject.id_subject).ToList();
                        foreach (var lesson in lessonTeacher)
                        {
                            lesson.id_teacher = 1;
                            lesson.id_subject = null;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Предмет не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
