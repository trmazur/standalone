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
        public MainViewModel(PatientSelectViewModel patientSelectViewModel)
        {
            PatientSelectViewModel = patientSelectViewModel;
        }

        public PatientSelectViewModel PatientSelectViewModel { get; }
    }
}
