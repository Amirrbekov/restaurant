using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Blog;

public class Post
{

    public int PostId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    [ValidateNever]
    public string Image { get; set; }
    public bool ShowInMain { get; set; } = true;
    public DateTime Created { get; set; } = DateTime.Now;

	[ValidateNever]
	public string ApplicationUserId { get; set; }
    [ValidateNever]
    [ForeignKey(nameof(ApplicationUserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }

}
