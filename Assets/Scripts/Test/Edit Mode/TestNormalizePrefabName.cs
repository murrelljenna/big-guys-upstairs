using NUnit.Framework;
using static game.assets.utilities.GameUtils;

namespace Tests
{
    public class TestNormalizePrefabName
    {
        [Test]
        public void TestTwoDigits()
        {
            string result = "Light Infantry(14)";

            Assert.AreEqual(
                "Light Infantry",
                normalizePrefabName(result)
            );
        }

        [Test]
        public void TestThreeDigits()
        {
            string result = "Light Infantry(144)";

            Assert.AreEqual(
                "Light Infantry",
                normalizePrefabName(result)
            );
        }

        [Test]
        public void TestClone()
        {
            string result = "Light Infantry(Clone)";

            Assert.AreEqual(
                "Light Infantry",
                normalizePrefabName(result)
            );
        }

    }
}
