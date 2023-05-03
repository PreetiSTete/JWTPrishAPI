using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace login_api.Model
{
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}