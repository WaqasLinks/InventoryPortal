//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DMSRepo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DynamicForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DynamicForm()
        {
            this.DynamicFormColumns = new HashSet<DynamicFormColumn>();
        }
    
        public int Id { get; set; }
        public string FormName { get; set; }
        public string TableName { get; set; }
        public bool IsAutoTruncate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DynamicFormColumn> DynamicFormColumns { get; set; }
    }
}