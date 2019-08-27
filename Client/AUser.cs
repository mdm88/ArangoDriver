﻿using System.Collections.Generic;

namespace ArangoDriver.Client
{
    public class AUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public Dictionary<string, object> Extra { get; set; }
        
        public AUser()
        {
            Active = true;
        }
    }
}
