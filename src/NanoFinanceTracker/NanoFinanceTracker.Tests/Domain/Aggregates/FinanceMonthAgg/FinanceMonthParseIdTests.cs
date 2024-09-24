using NanoFinanceTracker.Core.Domain.Aggregates.FinanceMonthAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Tests.Domain.Aggregates.FinanceMonthAgg
{
    public class FinanceMonthParseIdTests
    {
        [Fact]
        public void Should_ParseId_Successfully()
        {
            //Arrange
            string rawId = "#4-2024#2-09#7-user#id#7-account";

            //Act
            var parse = FinanceMonth.ParseFinanceMonthId(rawId);

            //Assert
            Assert.Equal(2024, parse.year);
            Assert.Equal(09, parse.month);
            Assert.Equal("user#id", parse.userId);
            Assert.Equal("account", parse.account);

        }

        [Fact]
        public void Should_ParseId_Fail()
        {
            //Arrange
            string rawId = "#5-2024#2-09#7-user#id#7-account";

            //Act
            var parse = FinanceMonth.ParseFinanceMonthId(rawId);

            //Assert
            Assert.Equal(0, parse.year);
            Assert.Equal(0, parse.month);
            Assert.Equal("", parse.userId);
            Assert.Equal("", parse.account);

        }
    }
}
