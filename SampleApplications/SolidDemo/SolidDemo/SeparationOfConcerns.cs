using System;
using System.Linq.Expressions;

namespace SolidDemo
{
    public class LoginController
    {
        private readonly IRepository repository;

        public LoginController(IRepository repository)
        {
            this.repository = repository;
        }

        public bool Login(string username, string password)
        {
            var user = repository.Find<User>(
                x => x.Name == username
                );
            
            if(user.LockedOut)
                return false;

            if(user.HashedPassword==HashPassword(password))
                return true;

            user.FailedLoginAttempts += 1;
            if(user.FailedLoginAttempts>3)
                user.LockedOut = true;

            return false;
        }

        private string HashPassword(string password)
        {
            return password.ToUpper();//not really secure
        }
    }

    public class User
    {
        public int FailedLoginAttempts { get; set; }
        public string Name { get; set; }
        public string HashedPassword { get; set; }
        public bool LockedOut { get; set; }
    }

    public interface IRepository
    {
        T Find<T>(Expression<Func<T,bool>> criteria);
    }
}