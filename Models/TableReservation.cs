using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class TableReservation
{
    [Key]
    public int Id { get; set; }
    [StringLength(50)]
	public string Name { get; set; } = null!;
	public string PhoneNumber { get; set; } = null!;

    public int NumberOfPeople { get; set; }

    public DateTime ArrivalDateTime { get; set; }
    public DateTime DepartureDateTime { get; set; }

    public bool IsActive { get; set; }

    public int TableId { get; set; }
    [ForeignKey(nameof(TableId))]
    [ValidateNever]
    public Table Table { get; set; }

    [NotMapped]
    public string ArrivalDate => ArrivalDateTime.ToString("MM/dd/yyyy");
    [NotMapped]
    public string ArrivalTime => ArrivalDateTime.ToString("hh:mm tt");
    [NotMapped]
    public string DepartureDate => DepartureDateTime.ToString("MM/dd/yyyy");
    [NotMapped]
    public string DepartureTime => DepartureDateTime.ToString("hh:mm tt");
}