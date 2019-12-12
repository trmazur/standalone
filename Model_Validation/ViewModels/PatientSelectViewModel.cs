using Model_Validation.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace Model_Validation.ViewModels
{
    public class PatientSelectViewModel:BindableBase
    {

        public string PatientID { get; set; }

        private Application _application;
        private IEventAggregator _eventAggregator;
        private Patient _patient;

        public ObservableCollection<Course> Courses { get; set; }
        public ObservableCollection<PlanSetup> Plans { get; set; }

        //PLAN
        private PlanSetup selectedPlan;
        public PlanSetup SelectedPlan
        {
            get { return selectedPlan; }
            set
            {
                SetProperty(ref selectedPlan, value);
                PlanSelected();
            }
        }

        //COURSE
        private Course selectedCourse;        
        public Course SelectedCourse
        {
            get { return selectedCourse; }
            set
            {
                SetProperty(ref selectedCourse, value);
                GetPlans();
            }
        }

        public DelegateCommand OpenPatientCommand { get; private set; }
        //CONSTRUCTOR
        public PatientSelectViewModel(VMS.TPS.Common.Model.API.Application application,IEventAggregator eventAggregator)
        {
            //Local variable
            _application = application;
            _eventAggregator = eventAggregator;

            Courses = new ObservableCollection<Course>();
            Plans = new ObservableCollection<PlanSetup>();
            OpenPatientCommand = new DelegateCommand(OnOpenPatient, CanOpenPatient);
        }

        //COMMAND - ON OPEN
        private void OnOpenPatient()
        {
            //Clearing course list when patient changse
            SelectedCourse = null;
            Courses.Clear();
            //Need to preface any OpenPatient command by ClosePatient (in case a patient is already open - scripting can only open one patient at a time)
            _application.ClosePatient();
            _patient = _application.OpenPatientById(PatientID);
            if (_patient != null)
            {
                //Populating course list for current patient
                foreach(var course in _patient.Courses)
                {
                    Courses.Add(course);
                }
            }
        }

        //CHECK - CAN OPEN
        private bool CanOpenPatient()
        {
            //Is patient ID not null?
            return true;            
        }

        //RETRIEVING PLANS UNDER COURSE
        private void GetPlans()
        {
            Plans.Clear();
            SelectedPlan = null;
            if (SelectedCourse != null)
            {
                foreach (var plan in SelectedCourse.PlanSetups)
                {
                    Plans.Add(plan);
                }
            }
        }

        private void PlanSelected()
        {
            if(SelectedPlan != null)
            {
                _eventAggregator.GetEvent<PlanSelectedEvent>().Publish(SelectedPlan);
            }            
        }

    }
}
