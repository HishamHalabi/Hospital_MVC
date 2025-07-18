﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Hospital.Models;

public partial class Surgery
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string SurgeryId { get; set; }


    public string PatientId { get; set; }

    public DateTime Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Type { get; set; }


    public  Patient? Patient { get; set; }

    public  string  RoomId { get; set; }
    public Room? Room { get; set; }


    public ICollection<NurseSurgery> NurseSurgeries { get; set; }

    public ICollection<DoctorSurgery> DoctorSurgeries { get; set; }


}