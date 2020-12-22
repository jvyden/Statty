using System;
using NUnit.Framework;
using StattyBot.util;

namespace StattyTests {
    public class UtilTests {
        [Test]
        public void GetsPositive() {
            Assert.True(Util.GetPositive(10) == "+");
            Assert.True(Util.GetPositive((long) 10) == "+");
        }
        
        [Test]
        public void GetsNegative() {
            Assert.True(Util.GetPositive(-10) == "");
            Assert.True(Util.GetPositive((long) -10) == "");
        }
        
        [Test]
        public void GetsPositiveStr() {
            Assert.True(Util.GetPositiveStr(10) == "+10");
            Assert.True(Util.GetPositiveStr((long) 10) == "+10");
        }
        
        [Test]
        public void GetsNegativeStr() {
            Assert.True(Util.GetPositiveStr(-10) == "-10");
            Assert.True(Util.GetPositiveStr((long) -10) == "-10");
        }
    }
}