using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly SQLiteAsyncConnection db;
        protected BaseRepository(SQLiteAsyncConnection connection)
        {
            db = connection;
        }
    }
}
