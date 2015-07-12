namespace LightBlue.MultiHost.Configuration
{
    public interface IServiceConfiguationService
    {
        void Add(string filePath);

        void Edit(string serviceTitle, string filePath);
    }

    public class ServiceConfiguationService : IServiceConfiguationService
    {
        public void Add(string filePath)
        {
            
        }

        public void Edit(string serviceTitle, string filePath)
        {

        }
    }
}