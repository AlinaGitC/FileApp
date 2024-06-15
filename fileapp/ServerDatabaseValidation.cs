using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileapp
{
    // Проверка правильности указания серверной базы данных
    class ServerDatabaseValidation : IDataValidation
    {
        public bool Validate(string data)
        {
            return data.StartsWith("Srvr=") && data.Contains("Ref=");
        }
    }
}
