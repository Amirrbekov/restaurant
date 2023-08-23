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
	public string Name { get; set; } = null!;
	public string PhoneNumber { get; set; } = null!;
	public DateTime Date { get; set; }
	public TimeSpan Time { get; set; }

    public int TableId { get; set; }
    [ForeignKey(nameof(TableId))]
    [ValidateNever]
    public Table Table { get; set; }
}
