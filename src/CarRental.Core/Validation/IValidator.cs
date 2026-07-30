namespace CarRental.Core.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T model);
}