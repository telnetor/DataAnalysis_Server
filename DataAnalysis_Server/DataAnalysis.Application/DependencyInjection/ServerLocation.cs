using Autofac;
using Autofac.Extensions.DependencyInjection;


namespace DataAnalysisFrame
{ 
    public class ServerLocation 
    {

        public static IContainer _iServiceProvider;

        public static void SetServerLocation(IContainer iServiceProvider)
        {
            _iServiceProvider = iServiceProvider;
        }
    }
}
