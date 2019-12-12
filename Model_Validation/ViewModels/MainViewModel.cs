using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_Validation.ViewModels
{
    public class MainViewModel:BindableBase
    {
        public MainViewModel(PatientSelectViewModel patientSelectViewModel, EclipseViewModel eclipseViewModel)
        {
            PatientSelectViewModel = patientSelectViewModel;
            EclipseViewModel = eclipseViewModel;
        }

        public PatientSelectViewModel PatientSelectViewModel { get; }
        public EclipseViewModel EclipseViewModel { get; }
    }
}
