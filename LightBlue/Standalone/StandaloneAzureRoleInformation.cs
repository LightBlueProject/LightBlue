namespace LightBlue.Standalone
{
    public class StandaloneAzureRoleInformation : IAzureRoleInformation
    {
        private readonly string _roleName;

        public StandaloneAzureRoleInformation(string roleName)
        {
            _roleName = roleName;
        }

        public string Name
        {
            get { return _roleName; }
        }
    }
}