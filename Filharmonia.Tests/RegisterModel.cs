using System.ComponentModel.DataAnnotations;
using Filharmonia.Areas.Identity.Pages.Account;

namespace Filharmonia.Tests
{
    public class RegisterModelTests
    {
        [Fact]
        public void RegisterModel_ShouldBeInvalid_WhenFieldsAreEmpty()
        {
            // Arrange
            var model = new RegisterModel.InputModel(); // Wszystkie pola puste

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);
            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Adres e-mail jest wymagany.");
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Hasło jest wymagane.");
        }

        [Fact]
        public void RegisterModel_ShouldBeValid_WhenAllFieldsAreCorrect()
        {
            // Arrange
            var model = new RegisterModel.InputModel
            {
                Email = "test@example.com",
                Password = "StrongPassword123!",
                ConfirmPassword = "StrongPassword123!"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);
            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void RegisterModel_ShouldBeInvalid_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var model = new RegisterModel.InputModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "WrongPassword!"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);
            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Hasło i potwierdzenie hasła muszą być takie same.");
     
        }
    }
}
