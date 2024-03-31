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
using static System.Net.Mime.MediaTypeNames;

namespace AudienceBusynessApp
{
    /// <summary>
    /// Логика взаимодействия для UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Window
    {
        public UserProfile()
        {
            InitializeComponent();

           
        }


        private void showNewInfo(object sender, DependencyPropertyChangedEventArgs e)
        {
            userInfoText.Text = UserInfo.surname + " " + UserInfo.name +" "+ UserInfo.lastname;
            login.Text = "Логин: " + UserInfo.login;
            if (UserInfo.isAdmin) { profesiya.Text = "Должность: Администратор"; } 
            else profesiya.Text = "Должность: Преподаватель";

            
            byte[] imageData;
            using (var db = new CollegeAuditoriumsEntities())
            {
                var teacher = db.Teachers.FirstOrDefault(t => t.id_teacher == UserInfo.teacher_id);
                if (teacher != null)
                {
                    imageData = teacher.profile_image;

                    // Преобразование массива байтов в BitmapImage    
                   if(imageData != null)
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
                }
            }
        }

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
                byte[] imageData;
                using (MemoryStream stream = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                    imageData = stream.ToArray();
                }

                using (var db = new CollegeAuditoriumsEntities())
                {
                    Teachers teacher = db.Teachers.FirstOrDefault(t => t.id_teacher == UserInfo.teacher_id); // Предполагая, что у вашей модели Teacher есть соответствующий столбец id
                    if (teacher != null)
                    {
                        teacher.profile_image = imageData;
                        db.SaveChanges();
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
            catch
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
