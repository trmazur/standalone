using Autofac;
using Model_Validation.Startup;
using Model_Validation.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Model_Validation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        VMS.TPS.Common.Model.API.Application app;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            app = VMS.TPS.Common.Model.API.Application.CreateApplication();
            if (app != null)
            {            
                var bootstrapper = new Bootstrapper();
                var container = bootstrapper.Bootstrap(app);
                var mainview = container.Resolve<MainView>();
                mainview.Show();
            }
        }
    }
}
