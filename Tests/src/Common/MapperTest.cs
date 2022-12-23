using FakeNewsBackend.Common;
using FakeNewsBackend.Common.Types;
using NUnit.Framework;

namespace Tests.Common
{
    internal class MapperTest
    {
        [TestCase("en", Language.EN)]
        [TestCase("en-us", Language.EN)]
        [TestCase("nl", Language.NL)]
        [TestCase("", Language.UNKNOWN)]
        [TestCase("jpa", Language.UNKNOWN)]
        public void GetLanguage(string languageInString, Language expectedResult)
        {
            var result = Mapper.GetLanguage(languageInString);

            Assert.AreEqual(result, expectedResult);
        }
    }
}
