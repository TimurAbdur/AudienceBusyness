//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AudienceBusynessApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lessons
    {
        public int lesson_number { get; set; }
        public int id_audience { get; set; }
        public int id_teacher { get; set; }
        public Nullable<int> id_subject { get; set; }
    
        public virtual Audiences Audiences { get; set; }
        public virtual Teachers Teachers { get; set; }
    }
}