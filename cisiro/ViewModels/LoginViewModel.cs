﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing.Constraints;

namespace cisiro.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }   

        public string Password { get; set; }    

        public bool RememberMe { get; set; }
    }
}
