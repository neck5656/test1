using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudentInformationManagementSystem.Models;
using System;

namespace StudentInformationManagementSystem.Tests.Models
{
    public class ModelValidationTests
    {
        [Fact]
        public void RegisterViewModel_ValidationRules_RequiredFieldsMissing()
        {
            // Arrange
            var model = new RegisterViewModel(); // Empty model
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("Username"));
            Assert.Contains(validationResults, r => r.MemberNames.Contains("Email"));
            Assert.Contains(validationResults, r => r.MemberNames.Contains("Password"));
            Assert.Contains(validationResults, r => r.MemberNames.Contains("FirstName"));
            Assert.Contains(validationResults, r => r.MemberNames.Contains("LastName"));
            Assert.Contains(validationResults, r => r.MemberNames.Contains("DateOfBirth"));
        }

        [Fact]
        public void Course_ValidationRules_RequiredFieldsMissing()
        {
            // Arrange
            var model = new Course(); // Empty model
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("CourseCode"));
            Assert.Contains(validationResults, r => r.MemberNames.Contains("CourseName"));
        }

        [Fact]
        public void Course_ValidationRules_ValidFieldsPassing()
        {
            // Arrange
            var model = new Course
            {
                CourseCode = "CS101",
                CourseName = "Introduction to Programming",
                Description = "Basic programming concepts",
                CreditHours = 3
            };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}