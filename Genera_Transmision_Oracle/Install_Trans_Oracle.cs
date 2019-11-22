using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Genera_Transmision_Oracle
{
    [RunInstaller(true)]
    public partial class Install_Trans_Oracle : System.Configuration.Install.Installer
    {
        public Install_Trans_Oracle()
        {
            InitializeComponent();
        }
    }
}
