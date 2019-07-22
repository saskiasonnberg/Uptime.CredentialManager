﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Uptime.CredentialManager.Web.Models;
using Uptime.CredentialManager.Web.ViewModels;

namespace Uptime.CredentialManager.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UptimeCredentialManagerWebContext _context;

        public UsersController(UptimeCredentialManagerWebContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetCredentials()
        {
            List<SelectListItem> credentials = _context.Credential.AsNoTracking()
                                                        .OrderBy(x => x.Description)
                                                        .Select(x =>
                                                        new SelectListItem
                                                        {
                                                            Text = x.Description,
                                                            Value = x.Id.ToString()
                                                        }).ToList();
            var credentialTip = new SelectListItem()
            {
                Text = "--- select credential ---",
                Value = null
            };

            credentials.Insert(0, credentialTip);
            return new SelectList(credentials, "Value", "Text");
        }
               

        // GET: Users
        public async Task<IActionResult> Index()
        {
          var users = await _context.User
                 .Include(x => x.UserCredentials)
                 .ThenInclude(x => x.Credential)
                 .ToListAsync();
          
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(x => x.UserCredentials)
                .ThenInclude(x => x.Credential)
                .FirstOrDefaultAsync(m => m.Id == id)
                ;
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var userVM = new UserEditViewModel();
            userVM.Credentials = GetCredentials();
           
            return View(userVM);
                  
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserEditViewModel userVM)
        {

            if (ModelState.IsValid)
            {
                var user = new User();
                {
                    user.Id = Guid.NewGuid();
                    user.Name = userVM.UserName;
                    user.UserCredentials = new List<UserCredential>();

                    var credential = _context.Find<Credential>(userVM.SelectedCredential);
                    user.UserCredentials.Add(new UserCredential { User = user, Credential = credential });
                    _context.Add(user);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(userVM);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(x => x.UserCredentials)
                .ThenInclude(x => x.Credential)
                .FirstOrDefaultAsync(m => m.Id == id)
                ;
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,UserCredential")] User user)
        {
        
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
           
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
