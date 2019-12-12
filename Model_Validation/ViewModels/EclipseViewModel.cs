using Model_Validation.Events;
using Model_Validation.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
using VMS.TPS.Common.Model.Types;

namespace Model_Validation.ViewModels
{
    public class EclipseViewModel:BindableBase
    {
        private Application _app;
        private IEventAggregator _eventAggregator;                
        public double[] depths = new double[] { 1.5, 5, 10, 20, 30 };
        private List<DoseProfileModel> profile_list;
        private Patient patient;

        public Patient Patient
        {
            get { return patient; }
            set
            {
                SetProperty(ref patient, value);
                GeneratePlanCommand.RaiseCanExecuteChanged();
            }
        }


        public PlotModel MyPlotModel { get; set; }

        public ObservableCollection<Beam> Beams { get; set; }

        public DelegateCommand GeneratePlanCommand { get; private set; }
        //public object OnPatientSelected { get; }

        public EclipseViewModel(IEventAggregator eventAggregator, Application app)
        {
            profile_list = new List<DoseProfileModel>();
            MyPlotModel = new PlotModel
            {
                Title = "Beam Profiles",
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop
            };
            SetAxes();

            _app = app;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PatientSelectedEvent>().Subscribe(OnPatientSelected);
            _eventAggregator.GetEvent<PlanSelectedEvent>().Subscribe(OnSelectBeamProfile);
            GeneratePlanCommand = new DelegateCommand(OnCalculatePlan, CanCalculatePlan);

        }

        private void OnPatientSelected(Patient patient)
        {
            Patient = patient;
        }

        private void OnCalculatePlan()
        {            
            if (Patient != null)
            {
                Patient.BeginModifications();
                var localCourse = Patient.AddCourse();
                var localPlan = localCourse.AddExternalPlanSetup(Patient.StructureSets.First());
                double[] fieldSizes = new double[] { 30, 40, 50, 177, 200 };
                ExternalBeamMachineParameters beamP = new ExternalBeamMachineParameters("HESN10", "6X", 600, "STATIC", null);
                foreach (var fs in fieldSizes)
                {
                    var localBeam = localPlan.AddStaticBeam(    beamP,
                                                                new VRect<double>(-1.0 * fs / 2.0, -1.0 * fs / 2.0, fs / 2.0, fs / 2.0),
                                                                0,
                                                                0,
                                                                0,
                                                                new VVector(0, -200, 0));
                }
                localPlan.SetPrescription(1, new DoseValue(100, DoseValue.DoseUnit.cGy), 1.0);
                localPlan.CalculateDose();
                _app.SaveModifications();
            }
        }

        private bool CanCalculatePlan()
        {
            //SHOULD CHECK THAT PATIENT ISN'T NULL, BUT MUST MAKE _patient A FULL PROPERTY SUCH THAT WE CAN IMPLEMENT RaiseCanExecuteChanged ON GeneratePlanCommand
            return true;
        }

        private void OnSelectBeamProfile(PlanSetup ps)
        {
            if (ps == null)
            {
                return;
            }          
            
            foreach(var beams in ps.Beams)
            {
                foreach(var depth in depths)
                {
                    VVector start = new VVector();
                    start.x = (beams.ControlPoints.First().JawPositions.X1 - 50) * (1000 + depth) / 1000;
                    start.y = depth - 200;
                    start.z = 0;

                    VVector stop = new VVector();
                    stop.x = (beams.ControlPoints.First().JawPositions.X2 + 50) * (1000 + depth) / 1000;
                    stop.y = depth - 200;
                    stop.z = 0;

                    double[] size = new double[Convert.ToInt16((stop.x-start.x+1))];
                    var doseProfile = beams.Dose.GetDoseProfile(start, stop, size);
                    var profile = new DoseProfileModel
                    {
                        FieldX = beams.ControlPoints.First().JawPositions.X2 - beams.ControlPoints.First().JawPositions.X1,
                        FieldY = beams.ControlPoints.First().JawPositions.Y2 - beams.ControlPoints.First().JawPositions.Y1,
                        Depth = depth
                    };

                    foreach(var point in doseProfile)
                    {

                        profile.DataPoints.Add(new Models.DataPoint { Position = point.Position.x, Value = point.Value });
                    }
                    profile_list.Add(profile);
                    
                }                
            }
            DrawProfiles();

        }

        private void DrawProfiles()
        {
            foreach (var profile_current in profile_list)
            {                

                LineSeries series = new LineSeries
                {
                    Title = $"{profile_current.FieldX} - {profile_current.Depth}"                    
                };
                foreach (var point in profile_current.DataPoints)
                {
                    series.Points.Add(new OxyPlot.DataPoint(point.Position, point.Value));
                }
                MyPlotModel.Series.Add(series);
            }
            MyPlotModel.InvalidatePlot(true);
        }

        private void SetAxes()
        {
            MyPlotModel.Axes.Add(new LinearAxis { Title = "Position [cm]", Position = AxisPosition.Bottom });
            MyPlotModel.Axes.Add(new LinearAxis { Title = "Dose [%]", Position = AxisPosition.Left });
        }


        //public DelegateCommand OpenPatientCommand { get; private set; }
        ////CONSTRUCTOR
        //public PatientSelectViewModel(VMS.TPS.Common.Model.API.Application application, IEventAggregator eventAggregator)
        //{
        //    //Local variable
        //    _application = application;
        //    _eventAggregator = eventAggregator;

        //    Courses = new ObservableCollection<Course>();
        //    Plans = new ObservableCollection<PlanSetup>();
        //    OpenPatientCommand = new DelegateCommand(OnOpenPatient, CanOpenPatient);
        //}


    }
}
