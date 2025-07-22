using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.ViewModels
{
    public class VMAbono : BaseViewModel
    {
        //private readonly ConexionService _conexionService;
        private readonly Musuarios? Usuario;
        public VMAbono(INavigation? navigation, Musuarios? usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
        }       

        public async Task SincronizarTodoAsync()
        {
            if (Usuario?.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            {
               await App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr); // Inserta nuevos clientes en el servidor 

                if (!string.IsNullOrEmpty(Usuario?.Usuario))
                {
                   await App.SQLiteDB.SincronizarCreditos(Usuario); // Inserta nuevos créditos en el servidor
                }
                else
                {
                    Console.WriteLine("⚠️ Error: Usuario.Usuario es null.");
                }

               
            }
        }
    }
}
