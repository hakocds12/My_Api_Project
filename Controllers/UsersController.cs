// Controllers/UsersController.cs - Changes for Try-Catch
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using System;
// using System.Linq; // Likely not needed anymore if using Dictionary
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static Dictionary<int, User> _users = new Dictionary<int, User>();
        private static int _nextId = 1;

        public UsersController()
        {
            if (_users.Count == 0)
            {
                var user1 = new User { Id = _nextId++, Name = "Abdel Yahia", Email = "hakoait@gmail.com" };
                var user2 = new User { Id = _nextId++, Name = "John Doe", Email = "bob@outlook.com" };
                _users.Add(user1.Id, user1);
                _users.Add(user2.Id, user2);
            }
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try // <-- Add Try block
            {
                return Ok(_users.Values);
            }
            catch (Exception ex) // <-- Add Catch block for any Exception
            {
                // Log the exception (in a real app, use a logging framework)
                Console.Error.WriteLine($"An error occurred in GetUsers: {ex}");
                // Return a standard 500 Internal Server Error
                return StatusCode(500, "An error occurred while retrieving users.");
            } // <-- End Catch block
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
             try // <-- Add Try block
            {
                if (_users.TryGetValue(id, out var user))
                {
                     return Ok(user);
                }
                else
                {
                    return NotFound(); // 404 Not Found is expected and NOT an exception to catch here
                }
            }
            catch (Exception ex) // <-- Add Catch block
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred in GetUser(id): {ex}");
                // Return a standard 500 Internal Server Error
                return StatusCode(500, $"An error occurred while retrieving user with ID {id}.");
            } // <-- End Catch block
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
             try // <-- Add Try block
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Bad Request is expected and NOT an exception to catch here
                }

                user.Id = _nextId++;
                _users.Add(user.Id, user);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex) // <-- Add Catch block
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred in CreateUser: {ex}");
                // Return a standard 500 Internal Server Error
                return StatusCode(500, "An error occurred while creating the user.");
            } // <-- End Catch block
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, User user)
        {
            try // <-- Add Try block
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Bad Request is expected and NOT an exception to catch here
                }

                 if (_users.TryGetValue(id, out var existingUser))
                {
                    existingUser.Name = user.Name;
                    existingUser.Email = user.Email;
                    return Ok(existingUser); // Or NoContent()
                }
                else
                {
                    return NotFound(); // 404 Not Found is expected and NOT an exception to catch here
                }
            }
            catch (Exception ex) // <-- Add Catch block
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred in UpdateUser(id): {ex}");
                 // Return a standard 500 Internal Server Error
                return StatusCode(500, $"An error occurred while updating user with ID {id}.");
            } // <-- End Catch block
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            try // <-- Add Try block
            {
                if (_users.Remove(id))
                {
                     return NoContent();
                }
                else
                {
                    return NotFound(); // 404 Not Found is expected and NOT an exception to catch here
                }
            }
            catch (Exception ex) // <-- Add Catch block
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred in DeleteUser(id): {ex}");
                 // Return a standard 500 Internal Server Error
                return StatusCode(500, $"An error occurred while deleting user with ID {id}.");
            } // <-- End Catch block
        }
    }
}      