using Models;
using Models.Blog;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository.Blog;

public interface IPosttRepository : IRepository<Post>
{
    void Update(Post post);
    public PostViewModel GetAllPostsPage(int pageNumber, string search);
}
