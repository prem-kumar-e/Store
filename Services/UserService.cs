// UserService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Store.Models;

public class UserService
{
    
    private readonly UserManager<AppUser> _userManager;   
    private readonly SignInManager<AppUser> _signInManager;
    private readonly NavigationManager _navigationManager;
    public UserService(SignInManager<AppUser> signInManager, NavigationManager navigationManager, UserManager<AppUser> userManager)
    {
        _userManager = userManager;
          _signInManager = signInManager;
        _navigationManager = navigationManager;
    }

    public async Task<IdentityResult> CreateUserAsync(string username, string email, string password)
    {
        var user = new AppUser { UserName = username, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Optional: Add user to a role (e.g., User)
            await _userManager.AddToRoleAsync(user, "User");
        }

        return result;
    }
    public async Task HandleLogin(string email, string password)
    {
        var httpContext = _signInManager.Context;
        if (httpContext.Response.HasStarted)
        {
            throw new InvalidOperationException("Response has already started. Cannot perform login.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Only navigate if the response hasn't started
            if (!httpContext.Response.HasStarted)
            {
                _navigationManager.NavigateTo("/");
            }
        }
        else
        {
            // Handle failed login attempt
            throw new Exception("Invalid login attempt.");
        }
    }
}
