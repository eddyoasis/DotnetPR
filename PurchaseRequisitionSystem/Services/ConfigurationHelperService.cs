namespace PurchaseRequisitionSystem.Services
{
    public interface IConfigurationHelperService
    {
        string GetApproverEmail(string key);
        string GetApproverName(string key);
        (string Approver, string Email, string Role) GetCostCenterApproverInfo(string costCenterName);
    }

    public class ConfigurationHelperService : IConfigurationHelperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationHelperService> _logger;

        public ConfigurationHelperService(IConfiguration configuration, ILogger<ConfigurationHelperService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GetApproverEmail(string key)
        {
            var email = _configuration[$"AppSettings:ApproverEmails:{key}"];

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning($" Email not found for {key}, using Default");
                email = _configuration["AppSettings:ApproverEmails:Default"];
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError($" Default email not configured");
                throw new InvalidOperationException("Default approver email not configured in appsettings.json");
            }

            return email;
        }

        public string GetApproverName(string key)
        {
            var name = _configuration[$"AppSettings:ApproverNames:{key}"];

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning($" Name not found for {key}, using key as name");
                return key;
            }

            return name;
        }

        public (string Approver, string Email, string Role) GetCostCenterApproverInfo(string costCenterName)
        {
            //var approver = _configuration[$"AppSettings:CostCenterApprovers:{costCenterName}:Approver"];
            var approver = "Approver";
            var email = _configuration[$"AppSettings:DepartmentRoles:{costCenterName}:Email"];
            var role = _configuration[$"AppSettings:DepartmentRoles:{costCenterName}:HOD"];

            if (string.IsNullOrEmpty(approver) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning($" Approver info not found for {costCenterName}, using defaults");
                return ("HOD", GetApproverEmail("Default"), "HOD");
            }

            return (approver, email, role);
        }
    }
}
