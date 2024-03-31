using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AudienceBusynessApp
{
    public static class WindowManager
    {
        public static MainForm mainForm = new MainForm();
        public static FreeUpAudience freeUpAudience = new FreeUpAudience();
        public static OccupyAudience occupyAudience = new OccupyAudience();
        public static SignInForm signInForm = new SignInForm();
        public static UserProfile userProfile = new UserProfile();
        public static AdminPanelForm adminPanelForm = new AdminPanelForm();
    }
}
