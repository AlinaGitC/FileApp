using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileapp
{
    // Проверка правильности указания пути к файловой базе данных
    class FileDatabaseValidation : IDataValidation
    {
        public bool Validate(string data)
        {
            return data.StartsWith("File=");
        }
    }
}
