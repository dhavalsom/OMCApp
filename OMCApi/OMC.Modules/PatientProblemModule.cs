namespace OMC.Modules
{
    using Ninject.Modules;
    using OMC.BL.Interface;
    using OMC.BL.Library;
    using OMC.DAL.Interface;
    using OMC.DAL.Library;

    public class PatientProblemModule : NinjectModule
    {
        public override void Load()
        {
            try
            {
                Bind<IPatientProblem>().To<PatientProblem>();
                Bind<IPatientProblemDataAccess>().To<PatientProblemDataAccess>();
            }
            catch
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
