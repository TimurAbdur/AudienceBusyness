using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для SignInForm.xaml
    /// </summary>
    public partial class SignInForm : Window
    {
        public SignInForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLogin.Text == "") MessageBox.Show("Вы не ввели логин! Пожалуйста повторите попытку!", "Ошибка!",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            else if (textBoxPassword.Password == "") MessageBox.Show("Вы не ввели пароль! Пожалуйста повторите попытку!", "Ошибка!",
                 MessageBoxButton.OK,
                 MessageBoxImage.Error);
            else
            {
                using (var BD = new CollegeAuditoriumsEntities())
                {
                    Teachers teacher = BD.Teachers.FirstOrDefault(u => u.login == textBoxLogin.Text && u.password == textBoxPassword.Password);
                    if(teacher != null) {
                        UserInfo.teacher_id = teacher.id_teacher;
                        UserInfo.login = teacher.login;
                        UserInfo.password = teacher.password;
                        UserInfo.name = teacher.name;
                        UserInfo.surname = teacher.surname;
                        UserInfo.lastname = teacher.lastname;
                        if(teacher.login == "Admin") UserInfo.isAdmin = true;
                        else UserInfo.isAdmin = false;
                        Hide();
                        WindowManager.mainForm.Show();
                    }
                    else
                        MessageBox.Show("Вы ввели не верный логин или пароль! Пожалуйста повторите попытку!", "Ошибка!",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
            WindowManager.mainForm.Show();
        }
    }
}
