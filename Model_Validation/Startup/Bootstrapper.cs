﻿using Autofac;
using Model_Validation.ViewModels;
using Model_Validation.Views;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace Model_Validation.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap(Application application)
        {
            var container = new ContainerBuilder();
            container.RegisterType<MainView>().AsSelf();
            //ViewModels
            container.RegisterType<MainViewModel>().AsSelf();
            container.RegisterType<PatientSelectViewModel>().AsSelf();
            //ESAPI data
            container.RegisterInstance(application);
            //Other data (SAYING THAT THERE WILL ONLY BE ONE EVENTAGGREGATOR THROUGHOUT PROJECT)            
            container.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            return container.Build();
        }
    }
}
