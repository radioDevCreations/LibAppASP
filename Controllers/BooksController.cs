﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using LibApp.Models;
using LibApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using LibApp.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;

        public BooksController(IBookRepository bookRepository, IGenreRepository genreRepository)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
        }
        [Authorize(Roles = "Owner, User, StoreManager")]
        public IActionResult Index()
        {
            var books = _bookRepository.GetBooks()
                .ToList();

            return View(books);
        }

        [Authorize(Roles = "Owner, User, StoreManager")]
        public IActionResult Details(int id)
        {
            var book = _bookRepository.GetBooks()
                .SingleOrDefault(b => b.Id == id);

            return View(book);
        }

        [Authorize(Roles = "Owner, StoreManager")]
        public IActionResult Edit(int id)
        {
            var book = _bookRepository.GetBooks().SingleOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookFormViewModel
            {
                Book = book,
                Genres = _genreRepository.GetGenres().ToList()
            };

            return View("BookForm", viewModel);
        }

        [Authorize(Roles = "Owner, StoreManager")]
        public IActionResult New()
        {
            var viewModel = new BookFormViewModel
            {
                Genres = _genreRepository.GetGenres().ToList()
            };

            return View("BookForm", viewModel);
        }

        [Authorize(Roles = "Owner, StoreManager")]
        [HttpPost]
        public IActionResult Save(Book book)
        {
            if (book.Id == 0)
            {
                book.DateAdded = DateTime.Now;
                _bookRepository.AddBook(book);
            }
            else
            {
                var bookInDb = _bookRepository.GetBooks().Single(b => b.Id == book.Id);
                bookInDb.Name = book.Name;
                bookInDb.AuthorName = book.AuthorName;
                bookInDb.GenreId = book.GenreId;
                bookInDb.ReleaseDate = book.ReleaseDate;
                bookInDb.DateAdded = book.DateAdded;
                bookInDb.NumberInStock = book.NumberInStock;
            }

            try
            {
                _bookRepository.Save();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Books");
        }
    }
}