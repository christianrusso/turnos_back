using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.Database.Enums
{
    public enum RubroEnum:int
    {
        [Description("Clinic")]
        Clinic = 1,

        [Description("Hairdressing")]
        Hairdressing = 2
    }
}
