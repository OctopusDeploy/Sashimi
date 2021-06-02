using Octopus.Server.MessageContracts.Features.Accounts;

namespace Sashimi.Server.Contracts.Accounts
{
    public abstract class AccountStoreContributor
    {
        public virtual bool CanContribute(AccountResource resource)
        {
            return false;
        }

        public virtual ValidationResult ValidateResource(AccountResource resource)
        {
            return ValidationResult.Success;
        }

        public virtual void ModifyResource(AccountResource accountResource, string name)
        {
        }

        public virtual void ModifyModel(AccountResource resource, AccountDetails model, string name)
        {
        }
    }

    public class ValidationResult
    {
        public static readonly ValidationResult Success = new ValidationResult(true, null);

        public static ValidationResult Error(string errorMessage)
        {
            return new ValidationResult(false, errorMessage);
        }

        ValidationResult(bool isValid, string? errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get; }
        public string? ErrorMessage { get; }
    }
}