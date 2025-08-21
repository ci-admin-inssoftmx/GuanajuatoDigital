using AdminUsuarios;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Components
{
    public class Menu : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var visibles = HttpContext.Session.GetString("menus");
            var menus = new List<MenuViewModel>()
                {
                    new MenuViewModel (){ Id=100, Titulo="Administracion",Nivel=0,Orden=1 },
                    new MenuViewModel (){ Id=200, Titulo="Licencias",Nivel=0, Orden=2},
                    new MenuViewModel (){ Id=101, Titulo="Usuarios",Accion="Inicio",Controlador="Usuario",Nivel=100, Orden=1 },
                    new MenuViewModel (){ Id=101, Titulo="Oficinas",Accion="Inicio",Controlador="Oficina",Nivel=100, Orden=2},
                };
            return View(menus);
        }
    }
}
