using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GuanajuatoAdminUsuarios.LoginController
{
    public static class AuthManager
    {

        private static List<LoginManager> Usuarios;



        public static LoginManager GetUser(string GUID)
        {
            if (Usuarios == null) return new LoginManager();
            var Usuario = Usuarios.Where(s => s.ID == GUID).FirstOrDefault();
            return Usuario;
        }

        public static LoginManager GetUsers(string username)
        {
            if (Usuarios == null) return new LoginManager();
            var Usuario = Usuarios.Where(s => s.User == username).FirstOrDefault();
            return Usuario;
        }

        public static void AddNewUser(string guid, string user)
        {
            if (Usuarios == null) Usuarios = new List<LoginManager>();
            Usuarios.Add(new LoginManager { ID = guid, User = user, LastConection = DateTime.Now, CanUse = true });
        }

              
        public static void SingOutUser(string user)
        {
            Func<LoginManager, LoginManager> g = (s) => {                 
                if(s.User == user)
                {
                    s.CanUse = false;
                }
                return s;                
                };

            if (Usuarios == null) Usuarios = new List<LoginManager>();

            Usuarios = Usuarios.Select(g).ToList();
        }

        public static void SingOutUserId(string Id)
        {

            Func<LoginManager, LoginManager> g = (s) => {

                if (s.ID == Id)
                {
                    s.CanUse = false;
                }
                return s;
            };

            if (Usuarios == null) Usuarios = new List<LoginManager>();

            Usuarios = Usuarios.Select(g).ToList();
        }


        public static void CleanUsers(string Id)
        {         
            if (Usuarios == null) Usuarios = new List<LoginManager>();

            Usuarios = Usuarios.Where(d=>d.CanUse).ToList();
        }

        public static void CleanUser(string GUID)
        {
            if (Usuarios == null) Usuarios = new List<LoginManager>();

            Usuarios = Usuarios.Where(d => d.ID==GUID && d.CanUse).ToList();
        }

    }


    public class LoginManager
    {
        public string ID { get; set; }
        public string User { get; set; }
        public bool CanUse { get; set; }

        public DateTime LastConection { get; set; }
    }


}
