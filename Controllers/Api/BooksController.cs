using AutoMapper;
using LibApp.Data;
using LibApp.Dtos;
using LibApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Web.Http;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;


namespace LibApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET /api/books
        [HttpGet]
        [Authorize(Roles = "User, StoreManager, Owner")]
        public IActionResult GetBooks()
        {
            var books = _context.Books
                                .Include(b => b.Genre)
                                .ToList()
                                .Select(_mapper.Map<Book, BookDto>);

            return Ok(books);
        }

        private ApplicationDbContext _context;
        private IMapper _mapper;
    }
}
