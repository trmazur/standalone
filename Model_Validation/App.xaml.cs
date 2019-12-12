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

[assembly: ESAPIScript(IsWriteable = true)]
namespace Model_Validation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        VMS.TPS.Common.Model.API.Application app;
        private Patient _patient;
        private Course _course;
        private PlanSetup _planSetup;

        private void Application_Startup(object sender, StartupEventArgs e)
        {            
            app = VMS.TPS.Common.Model.API.Application.CreateApplication();
            if (app != null)
            {            
                if (!String.IsNullOrEmpty(e.Args.FirstOrDefault()))
                {
                    _patient = app.OpenPatientById(e.Args[0].Split(';').First().Trim('"'));
                    _course = _patient.Courses.FirstOrDefault(x => x.Id == e.Args[0].Split(';')[1]);
                    _planSetup = _course.PlanSetups.FirstOrDefault(x => x.Id == e.Args[0].Split(';').Last().Trim('"'));
                }
                var bootstrapper = new Bootstrapper();
                var container = bootstrapper.Bootstrap(app,_patient,_course,_planSetup);
                var mainview = container.Resolve<MainView>();
                mainview.Show();
            }
        }
    }
}
