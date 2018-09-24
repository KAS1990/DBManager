//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBManager
{
    using System;
    using System.Collections.Generic;
    
    public partial class groups
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public groups()
        {
            this.falsestarts_rules = new HashSet<falsestarts_rules>();
            this.participations = new HashSet<participations>();
            this.round_dates = new HashSet<round_dates>();
        }
    
        public long id_group { get; set; }
        public string name { get; set; }
        public Nullable<int> start_year { get; set; }
        public Nullable<short> end_year { get; set; }
        public long desc { get; set; }
        public string second_col_name { get; set; }
        public Nullable<byte> from_1_qualif { get; set; }
        public Nullable<byte> from_2_qualif { get; set; }
        public Nullable<byte> round_after_qualif { get; set; }
        public string main_judge { get; set; }
        public string main_secretary { get; set; }
        public string row6 { get; set; }
        public string xml_file_name { get; set; }
        public byte sex { get; set; }
        public System.DateTime comp_start_date { get; set; }
        public Nullable<System.DateTime> comp_end_date { get; set; }
        public Nullable<int> round_finished_flags { get; set; }
    
        public virtual descriptions descriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<falsestarts_rules> falsestarts_rules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<participations> participations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<round_dates> round_dates { get; set; }
    }
}
