using System;
using TaskManager.API.Constants;
using TaskManager.API.Helpers;
using Xunit;

namespace TaskManager.UnitTests.API.Helpers
{
    public class HelperTest
    {
        public HelperTest() { }

        [Fact]
        public void It_Should_Return_True_If_Date_With_Valid_Format() {

            //Arrange
            var validDate = DateTime.Today.ToString(ApiConstants.DateFormat);

            //Act
            var response = Helper.IsValidDateFormat(validDate);

            //Assert
            Assert.True(response);
        }

        [Fact]
        public void It_Should_Return_False_If_Date_With_Invalid_Format()
        {

            //Arrange
            var invalidDate = "01011900";

            //Act
            var response = Helper.IsValidDateFormat(invalidDate);

            //Assert
            Assert.False(response);
        }
    }
}
