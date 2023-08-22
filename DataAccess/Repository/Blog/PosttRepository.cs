using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository.IRepository.Blog;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Blog;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Repository.Blog;

public class PosttRepository : Repository<Post>, IPosttRepository
{
    private readonly ApplicationDbContext _db;

    public PosttRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Post obj)
    {
        var objFromDb = _db.Posts.FirstOrDefault(x => x.PostId == obj.PostId);
        if (objFromDb != null)
        {
            objFromDb.Title = obj.Title;
            objFromDb.Body = obj.Body;
            objFromDb.ShowInMain = obj.ShowInMain;
            if (obj.Image != null)
            {
                objFromDb.Image = obj.Image;
            }
        }
    }

    public PostViewModel GetAllPostsPage(int pageNumber, string search)
    {
        int pageSize = 3;
        int skipAmount = pageSize * (pageNumber - 1);

        var query = _db.Posts.AsNoTracking().AsQueryable();

        if (!String.IsNullOrEmpty(search))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                                || EF.Functions.Like(x.Body, $"%{search}%"));

        int postsCount = query.Count();
        int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

        return new PostViewModel
        {
            PageNumber = pageNumber,
            PageCount = pageCount,
            NextPage = postsCount > skipAmount + pageSize,
            Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
            Search = search,
            Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
        };
    }
}
