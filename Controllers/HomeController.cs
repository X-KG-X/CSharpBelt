using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using belt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace belt.Controllers
{

    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost("register")]

        public IActionResult Register(IndexViewModel dataModel)
        {
            // Check initial ModelState
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == dataModel.NewUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("NewUser.Email", "Email already in use!");
                    
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher =new PasswordHasher<User>();
                    dataModel.NewUser.Password=Hasher.HashPassword(dataModel.NewUser, dataModel.NewUser.Password);
                    dbContext.Add(dataModel.NewUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId",(int)dataModel.NewUser.UserId);
                    return RedirectToAction("success");
                }
            }
            // other code
            return View("Index");
        } 


        [HttpPost("login")]
        public IActionResult Login(IndexViewModel dataModel)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == dataModel.NewLogin.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("NewLogin.Email", "Email is not registered");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<Login>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(dataModel.NewLogin, userInDb.Password, dataModel.NewLogin.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("NewLogin.Password", "Password mismatch");
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("UserId",userInDb.UserId);
                    return RedirectToAction("success");
                }
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            if (HttpContext.Session.GetInt32("UserId")!=null)
            {
                int? _UserId=HttpContext.Session.GetInt32("UserId");
                ViewBag.userId= (int) _UserId;

                User _user=dbContext.Users.FirstOrDefault(u=>u.UserId==(int)_UserId);
                ViewBag.user=_user;
                
                DateTime _currentTime= DateTime.Now;
                // ViewBag.currentTime=_currentTime;
          
                List<Event> _allEvents=dbContext.Events.Include(e=>e.Coordinator).Include(e=>e.Participants).OrderBy(e=>e.EventDate).Where(e=>e.EventDate>_currentTime).ToList();
                return View("Dashboard", _allEvents);
            }
            return View("Index");
        }

        [HttpGet("delete/{EventId}")]
        public IActionResult delete( int EventId)
        {
            Console.WriteLine("__________inside DELETE______");
            Event thisEvent=dbContext.Events.FirstOrDefault(w=>w.EventId==EventId);
            dbContext.Events.Remove(thisEvent);
            dbContext.SaveChanges();
            return RedirectToAction("success");
        }

        [HttpGet("leave/{EventId}")]
        public IActionResult leave(int EventId)
        {
            int? _userId=HttpContext.Session.GetInt32("UserId");
            Attendence leave=dbContext.Attendences.FirstOrDefault(a=>a.EventId==EventId&&a.UserId==(int)_userId);
            dbContext.Attendences.Remove(leave);
            dbContext.SaveChanges();
            return RedirectToAction("success");
        }

        [HttpGet("join/{EventId}")]
        public IActionResult join(int EventId)
        {
            int? _userId=HttpContext.Session.GetInt32("UserId");
            Attendence join= new Attendence();
            join.UserId=(int)_userId;
            join.EventId=EventId;
            dbContext.Add(join);
            dbContext.SaveChanges();
            return RedirectToAction("success");
        }
        
        [HttpGet("newActivity")]
        public IActionResult newActivity()
        {
            return View("NewActivity");
        }

        [HttpPost("addEvent")]
        public IActionResult addEvent(Event dataModel)
        {
            if(ModelState.IsValid)
            {
                int? _userId=HttpContext.Session.GetInt32("UserId");
                dataModel.UserId=(int)_userId;

                dataModel.Coordinator=dbContext.Users.FirstOrDefault(u=>u.UserId==(int)_userId);

                dbContext.Add(dataModel);
                dbContext.SaveChanges();
                Event _thisEvent=dbContext.Events.FirstOrDefault(e=>e.Title==dataModel.Title);
                return RedirectToAction("oneEvent", new{eventId=_thisEvent.EventId});
            }
            else 
            {
                return View("NewActivity");
            }

        }

        [HttpGet("oneEvent/{eventId}")]
        public IActionResult oneEvent(int eventId)
        {
            // Console.WriteLine("sdugsudglasuid"+eventId);
            Event _thisEvent=dbContext.Events.Include(e=>e.Coordinator).Include(e=>e.Participants).ThenInclude(p=>p.User)
            .FirstOrDefault(e=>e.EventId==eventId);

            int? _userId=HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId=(int)_userId;
            
            return View("Event",_thisEvent);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
