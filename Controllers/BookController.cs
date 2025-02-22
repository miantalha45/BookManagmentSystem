using Microsoft.AspNetCore.Mvc;
using BookManagmentSystem.Models;
using BookManagmentSystem.Data.Repositories;
using BookManagmentSystem.Data;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookManagmentSystem.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetBookDetails/{ID}")]
        public async Task<IActionResult> GetBookDetails(int ID)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(ID);
            if (book == null)
                return NotFound("No Book Found");

            book.ViewsCount += 1;
            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { BookDetails = book });
        }

        [HttpGet("GetBooksByPopularity")]
        public async Task<IActionResult> GetBooksByPopularity(int pageNumber = 1, int pageSize = 20)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            if (books == null)
                return NotFound();

            foreach (var book in books)
            {
                book.PopularityCount = book.ViewsCount * 0.5 + book.PublicationYear * 2;
            }
            books = books.OrderByDescending(b => b.PopularityCount).Skip((pageNumber - 1) * pageSize).Take(pageSize);


            return Ok(books.Select(b => b.Title));
        }

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook([FromBody] Books book)
        {
            if (book == null)
            {
                return BadRequest("Book Data is null.");
            }

            var existingBook = await _unitOfWork.Books.FirstOrDefaultAsync(b => b.Title == book.Title);
            if (existingBook != null)
            {
                return BadRequest("Book with the same title already exists.");
            }

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return Ok(book);
        }

        [HttpPost("AddBooksInBulk")]
        public async Task<ActionResult<IEnumerable<Books>>> AddBooks([FromBody] IEnumerable<Books> books)
        {
            if (books == null || !books.Any())
            {
                return BadRequest("Books Data is null or empty.");
            }

            var bookTitles = books.Select(b => b.Title).ToHashSet();

            var existingTitles = (await _unitOfWork.Books.GetAllAsync())
                .Where(b => bookTitles.Contains(b.Title))
                .Select(b => b.Title)
                .ToHashSet();

            var duplicateBooks = books.Where(b => existingTitles.Contains(b.Title)).ToList();
            if (duplicateBooks.Any())
            {
                return BadRequest($"Books with the following titles already exist: {string.Join(", ", duplicateBooks.Select(b => b.Title))}");
            }

            await _unitOfWork.Books.AddRangeAsync(books);
            await _unitOfWork.SaveChangesAsync();

            return Ok(books);
        }

        [HttpPut("UpdateBook/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Books book)
        {
            var existingBook = await _unitOfWork.Books.GetByIdAsync(id);
            if (existingBook == null)
                return NotFound();

            existingBook.Title = book.Title;
            existingBook.AuthorName = book.AuthorName;

            _unitOfWork.Books.Update(existingBook);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Book Updated Successfully.");
        }

        [HttpDelete("DeleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            _unitOfWork.Books.Delete(book);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Deleted Successfully.");
        }

        [HttpDelete("DeleteBooksInBulk")]
        public async Task<IActionResult> DeleteBooksInBulk([FromBody] IEnumerable<int> bookIds)
        {
            if (bookIds == null || !bookIds.Any())
            {
                return BadRequest("No book IDs provided.");
            }

            var booksToDelete = (await _unitOfWork.Books.GetAllAsync())
                .Where(b => bookIds.Contains(b.ID))
                .ToList();

            if (!booksToDelete.Any())
            {
                return NotFound("No books found for the given IDs.");
            }

            _unitOfWork.Books.DeleteRange(booksToDelete);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Books deleted successfully.");
        }
    }
}