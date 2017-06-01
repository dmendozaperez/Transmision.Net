using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace Genera_Transmision
{
    [RunInstaller(true)]
    public partial class Install_Trans : System.Configuration.Install.Installer
    {
        public Install_Trans()
        {
            InitializeComponent();
        }
    }
}
