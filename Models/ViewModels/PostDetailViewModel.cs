using Models.Blog;
using Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels;

public class PostDetailViewModel
{
	public Post Post { get; set; }
    public Comments Comments { get; set; }
    public List<Comments> CommentList { get; set; }
}
