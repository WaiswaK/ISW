//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISW.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Leave
    {
        public string Leave_ID { get; set; }
        public string Employee_ID { get; set; }
        public System.DateTime Start_Date { get; set; }
        public System.DateTime End_Date { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
