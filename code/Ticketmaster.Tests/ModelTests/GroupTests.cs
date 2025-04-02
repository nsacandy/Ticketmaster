using System.ComponentModel.DataAnnotations;
using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;


    public class GroupTests
    {
        [Fact]
        public void Can_Create_Group_With_Valid_Data()
        {
            // Arrange
            var group = new Group
            {
                GroupId = 1,
                GroupName = "IT Support",
                ManagerId = 101,
                EmployeeIds = "1,2,3"
            };

            // Act & Assert
            Assert.Equal(1, group.GroupId);
            Assert.Equal("IT Support", group.GroupName);
            Assert.Equal(101, group.ManagerId);
            Assert.Equal("1,2,3", group.EmployeeIds);
        }

        [Fact]
        public void GroupName_Should_Have_Required_And_StringLength_Attributes()
        {
            // Arrange
            var property = typeof(Group).GetProperty(nameof(Group.GroupName));
            var requiredAttr = property.GetCustomAttributes(typeof(RequiredAttribute), false);
            var stringLengthAttr = property.GetCustomAttributes(typeof(StringLengthAttribute), false);

            // Assert
            Assert.Single(requiredAttr);
            Assert.Single(stringLengthAttr);

            var length = (StringLengthAttribute)stringLengthAttr[0];
            Assert.Equal(100, length.MaximumLength);
        }

        [Fact]
        public void Manager_NavigationProperty_Can_Be_Null()
        {
            // Arrange
            var group = new Group
            {
                GroupId = 2,
                GroupName = "Marketing",
                Manager = null
            };

            // Act & Assert
            Assert.Null(group.Manager);
        }

        [Fact]
        public void EmployeeIds_Can_Be_Null_Or_Empty()
        {
            // Arrange
            var group1 = new Group { EmployeeIds = null };
            var group2 = new Group { EmployeeIds = "" };

            // Assert
            Assert.Null(group1.EmployeeIds);
            Assert.Equal("", group2.EmployeeIds);
        }
    }
