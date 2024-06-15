using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileapp
{
    internal interface IDataValidation
    {
        bool Validate(string data);
    }
}
