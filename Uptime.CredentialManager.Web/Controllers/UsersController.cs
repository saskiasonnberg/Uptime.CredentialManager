using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public IEnumerable<SelectListItem> GetCredentials(Expression<Func<Credential, bool>> predicate)
        {
            List<SelectListItem> credentials = _context.Credential
                                                       .Include(x => x.UserCredentials)
                                                       .Where(predicate)
                                                       .OrderBy(x => x.Description)
                                                       .Select(x =>
                                                            new SelectListItem
                                                            {
                                                                Text = x.Description,
                                                                Value = x.Id.ToString()
                                                            })
                                                       .ToList();
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
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _context.User.Include(x => x.UserCredentials)
                                          .ThenInclude(x => x.Credential)
                                          .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            
            var userVM = new UserEditViewModel();
            {
                userVM.UserId = user.Id;
                userVM.UserName = user.Name;
                userVM.UserRole = user.Role;
                userVM.CredentialList = user.UserCredentials.Select(x => new CredentialViewModel
                {
                    Id = x.CredentialId,
                    Description = x.Credential.Description
                }).ToList();
            };
            return View(userVM);
        }


        // GET: Users/Create  
        [Authorize("IsAdmin")]
        public IActionResult Create()
        {
            var userVM = new UserEditViewModel();
            userVM.Credentials = GetCredentials(x => true);           
            return View(userVM);                  
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize("IsAdmin")]
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
                    user.Role = userVM.UserRole;
                    user.UserCredentials = new List<UserCredential>();

                    foreach (Guid credentialId in userVM.SelectedCredential)
                    {
                        var credential = _context.Find<Credential>(credentialId);
                        user.UserCredentials.Add(new UserCredential { User = user, Credential = credential });
                        _context.Add(user);
                    }    
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(userVM);
        }

        // GET: Users/Edit/5   
        [Authorize("IsAdmin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var user = await _context.User.Include(x => x.UserCredentials)
                                          .ThenInclude(x => x.Credential)
                                          .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }            

            var unusedCredentials = await _context.Credential                                                  
                                                  .Where(x => !IsCredentialUnderUser(x, user))
                                                  .OrderBy(x => x.Description)
                                                  .Select(x => new SelectListItem
                                                  {
                                                      Text = x.Description,
                                                      Value = x.Id.ToString()
                                                  })
                                                  .ToListAsync();
            
            var credentialTipp = new SelectListItem()
            {
                Text = "--- select credential ---",
                Value = null
            };

            unusedCredentials.Insert(0, credentialTipp);
            var dropdown = new SelectList(unusedCredentials, "Value", "Text");                       

            var userVM = new UserEditViewModel();
            {                
                userVM.UserId = user.Id;
                userVM.UserName = user.Name;
                userVM.UserRole = user.Role;

                userVM.CredentialList = user.UserCredentials.Select(x => new CredentialViewModel
                {
                    Id = x.CredentialId,
                    Description = x.Credential.Description
                }).ToList();

                userVM.Credentials = dropdown;
            };                     
            return View(userVM);
        }

        private bool IsCredentialUnderUser(Credential credential, User user)
        {
            return user.UserCredentials.Any(x => x.CredentialId == credential.Id);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize("IsAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.User
                                             .Include(x => x.UserCredentials)
                                             .FirstOrDefaultAsync(m => m.Id == userVM.UserId);
                    {                       
                        user.Name = userVM.UserName;                        


                        foreach (Guid credentialId in userVM.SelectedCredential)
                        {
                            var credential = _context.Find<Credential>(credentialId);
                            user.UserCredentials.Add(new UserCredential { User = user, Credential = credential });
                        }                                               
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userVM.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", new { id = userVM.UserId });
            }                       
            return View(userVM);
        }

        // GET: Users/Delete/5   
        [Authorize("IsAdmin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [Authorize("IsAdmin")]
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
        
        
        [Authorize("IsAdmin")]
        public async Task<IActionResult> Remove(Guid userId, Guid credentialId)
        {
            
            var user = await _context.User.Include(x => x.UserCredentials)
                                          .ThenInclude(x => x.Credential)
                                          .FirstOrDefaultAsync(m => m.Id == userId);

            var userCredential = user.UserCredentials.FirstOrDefault(x => x.CredentialId == credentialId);
            user.UserCredentials.Remove(userCredential);                       
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = userId });
        }   
    }
}
