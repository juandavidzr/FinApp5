using FinApp5.Modelo;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Repositories
{
    public class VMClientesRepository: BaseRepository
    {
        public VMClientesRepository(SQLiteAsyncConnection connection) : base(connection)
        {
            db.CreateTableAsync<Mcliente>().Wait();
        }
    }
}
