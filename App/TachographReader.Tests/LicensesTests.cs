using NUnit.Framework;
using System;
using System.Globalization;
using Webcal.Shared;

namespace TachographReader.Tests
{
    [TestFixture]
    public class LicensesTests
    {
        [Test]
        public void IsValid_SerialIsEmpty_ReturnsFalse()
        {
            string serial = string.Empty;
            DateTime expirationDate;

            bool isValid = LicenseManager.IsValid(serial, out expirationDate);

            Assert.That(isValid == false);
        }

        [Test]
        public void IsValid_SerialContainsLetters_ReturnsFalse()
        {
            string serial = "1234A5678";
            DateTime expirationDate;

            bool isValid = LicenseManager.IsValid(serial, out expirationDate);

            Assert.That(isValid == false);
        }

        [Test]
        public void IsValid_SerialIsNotAValidLong_ReturnsFalse()
        {
            string serial = "9223372036885487758808"; //Greater than int64.maxvalue
            DateTime expirationDate;

            bool isValid = LicenseManager.IsValid(serial, out expirationDate);

            Assert.That(isValid == false);
        }

        [Test]
        public void IsValid_InvalidSerial_ReturnsFalse()
        {
            string serial = "999999999999999999"; 
            DateTime expirationDate;

            bool isValid = LicenseManager.IsValid(serial, out expirationDate);

            Assert.That(isValid == false);
        }

        [Test]
        public void IsValid_ValidSerial_ReturnsTrueAndTodayAsExpirationDate()
        {
            DateTime now = DateTime.Parse(DateTime.Now.ToString("dd-MMM-yyyy"));

            string serial = now.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(Char.Parse("0"));
            DateTime expirationDate;

            bool isValid = LicenseManager.IsValid(serial, out expirationDate);

            Assert.That(isValid);
            Assert.AreEqual(expirationDate, now);
        }

        [Test]
        public void GetExpirationDate_SerialIsEmpty_ReturnsNull()
        {
            string serial = string.Empty;

            DateTime? expirationDate = LicenseManager.GetExpirationDate(serial);

            Assert.IsNull(expirationDate);
        }

        [Test]
        public void GetExpirationDate_SerialContainsLetters_ReturnsNull()
        {
            string serial = "1234A5678";

            DateTime? expirationDate = LicenseManager.GetExpirationDate(serial);

            Assert.IsNull(expirationDate);
        }

        [Test]
        public void GetExpirationDate_SerialIsNotAValidLong_ReturnsNull()
        {
            string serial = "9223372036885487758808"; //Greater than int64.maxvalue
           
            DateTime? expirationDate = LicenseManager.GetExpirationDate(serial);

            Assert.IsNull(expirationDate);
        }

        [Test]
        public void GetExpirationDate_InvalidSerial_ReturnsNull()
        {
            string serial = "999999999999999999";
            
            DateTime? expirationDate = LicenseManager.GetExpirationDate(serial);

            Assert.IsNull(expirationDate);
        }

        [Test]
        public void GetExpirationDate_ValidSerial_ReturnsTrueAndTodayAsExpirationDate()
        {
            DateTime now = DateTime.Parse(DateTime.Now.ToString("dd-MMM-yyyy"));
            string serial = now.Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd(Char.Parse("0"));
            
            DateTime? expirationDate = LicenseManager.GetExpirationDate(serial);

            Assert.IsNotNull(expirationDate);
            Assert.AreEqual(expirationDate, now);
        }

        [Test]
        public void HasExpired_DateTimeLessThanNow_ReturnsTrue()
        {
            DateTime expirationDate = DateTime.Now.AddDays(-1);

            bool hasExpired = LicenseManager.HasExpired(expirationDate);

            Assert.True(hasExpired);
        }

        [Test]
        public void HasExpired_DateTimeEqualsNow_ReturnsFalse()
        {
            DateTime expirationDate = DateTime.Now;

            bool hasExpired = LicenseManager.HasExpired(expirationDate);

            Assert.False(hasExpired);
        }

        [Test]
        public void HasExpired_DateTimeGreaterThanNow_ReturnsFalse()
        {
            DateTime expirationDate = DateTime.Now.AddDays(1);

            bool hasExpired = LicenseManager.HasExpired(expirationDate);

            Assert.False(hasExpired);
        }
    }
}
