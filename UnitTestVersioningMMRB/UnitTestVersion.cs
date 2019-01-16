using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersioningMMRB.VersionInterface;

namespace UnitTestVersioningMMRB
{
	[TestClass]
	public class UnitTestVersion
	{
		[TestMethod]
		public void TestOverrides()
		{
			Version version = new Version(1, 2, 3, 4);
			Assert.AreEqual("1.2.3.4", version.ToString());
		}
		[TestMethod]
		public void TestInrements()
		{
			Version version = new Version();
			Assert.AreEqual("0.0.0.1", version.ToString());
			version.IncrementBuild();
			Assert.AreEqual("0.0.0.2", version.ToString());
			version.IncrementRelease();
			Assert.AreEqual("0.0.1.0", version.ToString());
			version.IncrementMinor();
			Assert.AreEqual("0.1.0.0", version.ToString());
			version.IncrementMajor();
			Assert.AreEqual("1.0.0.0", version.ToString());
			version.IncrementMinor();
			Assert.AreEqual("1.1.0.0", version.ToString());
			version.IncrementRelease();
			Assert.AreEqual("1.1.1.0", version.ToString());
			version.IncrementBuild();
			Assert.AreEqual("1.1.1.1", version.ToString());
			version.IncrementMajor();
			Assert.AreEqual("2.0.0.0", version.ToString());
		}
	}
}
