using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudienceBusynessApp
{
    public static class UserInfo
    {
        public static int teacher_id = 0;
        public static string login = null;
        public static string password = null;
        public static string name = null;
        public static string surname = null;
        public static string lastname = null;

        //Переменная нужна для того чтобы каждый раз когда показывая окно занять ауд не обновлялся список предметов преподавателя
        public static bool isShowSubjects = false;
        //Узнаем админ ли это
        public static bool isAdmin = false;
    }
}
