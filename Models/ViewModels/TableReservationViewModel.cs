using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels;

public class TableReservationViewModel
{
    public TableReservation TableReservation { get; set; }
    public IEnumerable<SelectListItem> TableList { get; set; }
}
