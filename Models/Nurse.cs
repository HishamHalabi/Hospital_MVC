﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Hospital.Models;

public partial class Nurse
{
    public string NurseId { get; set; }

    public string Name { get; set; }

    public string Gender { get; set; }

    public string Address { get; set; }

    public string DepartmentId { get; set; }

    public  Department? Department { get; set; }


    public ICollection<NurseSurgery>  NurseSurgeries { get; set; }


}