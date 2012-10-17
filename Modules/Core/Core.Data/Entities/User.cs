using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Core.Data.Entities
{
    public class User
    {
        public virtual int Id { get; protected set; }
        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual Role Role { get; set; }
        public User()
        {
            Role = new Role();
        }
    }
}
