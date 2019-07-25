﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Uptime.CredentialManager.Web.Models;
using Uptime.CredentialManager.Web.ViewModels;

namespace Uptime.CredentialManager.Web.Controllers
{
    public class CredentialsController : Controller
    {
        private readonly UptimeCredentialManagerWebContext _context;

        public CredentialsController(UptimeCredentialManagerWebContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetUsers(Expression<Func<User, bool>> predicate)
        {
            List<SelectListItem> users = _context.User
                                                 .Include(x => x.UserCredentials)
                                                 .Where(predicate)
                                                 .OrderBy(x => x.Name)
                                                 .Select(x =>
                                                    new SelectListItem
                                                    {
                                                        Text = x.Name,
                                                        Value = x.Id.ToString()
                                                    }).ToList();
            var credentialTip = new SelectListItem()
            {
                Text = "--- select user ---",
                Value = null
            };

            users.Insert(0, credentialTip);
            return new SelectList(users, "Value", "Text");
        }
        
        // GET: Credentials
        public async Task<IActionResult> Index()
        {
            return View(await _context.Credential.ToListAsync());
        }

        // GET: Credentials/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credential.Include(x => x.UserCredentials)
                                                      .ThenInclude(x => x.User)
                                                      .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            var credentialVM = new CredentialEditViewModel();
            {
                credentialVM.Id = credential.Id;
                credentialVM.Description = credential.Description;
                credentialVM.Value = credential.Value;
                credentialVM.UserList = credential.UserCredentials.Select(x => new UserViewModel
                {
                    UserId = x.UserId,
                    UserName = x.User.Name
                }).ToList();
            };

            return View(credentialVM);
        }

        // GET: Credentials/Create
        public IActionResult Create()
        {
            var credentialVM = new CredentialEditViewModel();
            credentialVM.Users = GetUsers(x => true);

            return View(credentialVM);
        }

        // POST: Credentials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CredentialEditViewModel credentialVM)
        {
            if (ModelState.IsValid)
            {
               var credential = new Credential();
                {
                    credential.Id = Guid.NewGuid();
                    credential.Description = credentialVM.Description;
                    credential.Value = credentialVM.Value;                         
             
                    credential.UserCredentials = new List<UserCredential>();
                                      
                    foreach (Guid userId in credentialVM.SelectedUser)
                    {
                        var user = _context.Find<User>(userId);
                        credential.UserCredentials.Add(new UserCredential { User = user, Credential = credential });
                        _context.Add(credential);
                    }
                    

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                                                          
            }
            return View(credentialVM);
        }

        // GET: Credentials/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credential.Include(x => x.UserCredentials)
                                                      .ThenInclude(x => x.User)
                                                      .FirstOrDefaultAsync(m => m.Id == id);           
                                               
            if (credential == null)
            {
                return NotFound();
            }

            var credentialVM = new CredentialEditViewModel();
            {
                credentialVM.Id = credential.Id;
                credentialVM.Description= credential.Description;
                credentialVM.Value = credential.Value;
                credentialVM.UserList = credential.UserCredentials.Select(x => new UserViewModel
                {
                    UserId = x.UserId,
                    UserName = x.User.Name
                }).ToList();

                credentialVM.Users = GetUsers(x => !x.UserCredentials.Any(y => x.Id == y.UserId));
            };
           
            return View(credentialVM);
        }

        // POST: Credentials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CredentialEditViewModel credentialVM)
        {
           if (ModelState.IsValid)
            {
                try
                {
                    var credential = await _context.Credential.Include(x => x.UserCredentials).FirstOrDefaultAsync(m => m.Id == credentialVM.Id);
                    {
                        credential.Description = credentialVM.Description;
                        credential.Value = credentialVM.Value;

                        foreach (Guid userId in credentialVM.SelectedUser)
                        {
                            var user = _context.Find<User>(userId);
                            credential.UserCredentials.Add(new UserCredential { User = user, Credential = credential });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialExists(credentialVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", new { id = credentialVM.Id });
                //return RedirectToAction(nameof(Index));
            }
            return View(credentialVM);
        }

        // GET: Credentials/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credential
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            return View(credential);
        }

        // POST: Credentials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var credential = await _context.Credential.FindAsync(id);
            _context.Credential.Remove(credential);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet("{userId}/{credentialId}")]
        public async Task<IActionResult> Remove(Guid userId, Guid credentialId)
        {

            var credential = await _context.Credential.Include(x => x.UserCredentials)
                                          .ThenInclude(x => x.User)
                                          .FirstOrDefaultAsync(m => m.Id == credentialId);

            var userCredential = credential.UserCredentials.FirstOrDefault(x => x.UserId == userId);

            credential.UserCredentials.Remove(userCredential);

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = credentialId });
        }

        private bool CredentialExists(Guid id)
        {
            return _context.Credential.Any(e => e.Id == id);
        }
    }
}
