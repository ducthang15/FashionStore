using FashionStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    protected readonly fashionDbContext _context;

    public BaseController(fashionDbContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
      
        ViewBag.Categories = _context.Categories.ToList();

        base.OnActionExecuting(context);
    }
}