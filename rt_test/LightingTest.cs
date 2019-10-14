using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;

namespace rt_test
{
    [TestClass]
    public class LightingTest
    {
        [TestMethod]
        public void APointLightHasAPositionAndIntensity()
        {
            var intensity = new Color(1, 1, 1);
            var position = new Point(0, 0, 0);
            var light = new PointLight(position, intensity);
            Assert.AreEqual(intensity, light.Intensity);
            Assert.AreEqual(position, light.Position);
        }
    }
}
