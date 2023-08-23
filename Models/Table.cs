using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class Table
{
    [Key]
    public int Id { get; set; }
    public int Seats { get; set; }


    public int RestaurantGroupId { get; set; }
    [ForeignKey(nameof(RestaurantGroupId))]
    [ValidateNever]
    public RestaurantsGroup RestaurantGroup { get; set; }

}
