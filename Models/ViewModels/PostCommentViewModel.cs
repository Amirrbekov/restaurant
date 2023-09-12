﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels;

public class PostCommentViewModel
{
    public Comments Comments { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> PostList { get; set; }
}
