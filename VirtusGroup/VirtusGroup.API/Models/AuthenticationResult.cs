using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtusGroup.API.Models
{
    public class AuthenticationResult
    {
        public User User { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess => User != null;
    }
}