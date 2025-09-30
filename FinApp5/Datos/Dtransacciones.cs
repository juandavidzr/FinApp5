using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.ViewModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Datos
{
    public class Dtransacciones : BaseViewModel
    {
        SqlCommand cmd = new SqlCommand();

        //private bool ValidarPermisos(Mgasto mgasto)
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand("permisoGastos", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRutaAc", mgasto.strCodigoRuta);
        //        CONEXIONMAESTRA.Abrir();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}
    }
}
