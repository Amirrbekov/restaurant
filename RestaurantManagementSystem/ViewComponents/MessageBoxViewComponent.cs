using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.Comment;
using System.Security.Claims;
using Utility;

namespace RestaurantManagementSystem.ViewComponents;

public class MessageBoxViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    public MessageBoxViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var newMessages = _unitOfWork.Contact.GetAll();
        
        return View(newMessages);
    }
}
