using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for YamlReadTest
    /// </summary>
    [TestFixture]
    public class YamlReadTest
    {
        private string _testFilesPath;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            _testFilesPath = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks.Tests\Yaml");
        }

        [Test(Description="Read information from YAML file")]
        public void YamlReadExecute_SimpleYamlDocument()
        {
            YamlRead task = new YamlRead();
            task.BuildEngine = new MockBuild();
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.YamlFileName = Path.Combine(_testFilesPath, @"build1.yml");
            task.Path = "version";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("1.0", task.Value);
        }

        [Test(Description = "Read information from YAML file")]
        public void YamlReadExecute_StructuredYamlDocument1()
        {
            YamlRead task = new YamlRead();
            task.BuildEngine = new MockBuild();
            task.YamlFileName = Path.Combine(_testFilesPath, @"build2.yml");
            task.Path = "environment.access_token.secure";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("c33b17442180b40749a9d6f82b909902295a542b2a7fb7228647abbb689958b0", task.Value);
        }

        [Test(Description = "Read information from YAML file")]
        public void YamlReadExecute_StructuredYamlDocument2()
        {
            YamlRead task = new YamlRead();
            task.BuildEngine = new MockBuild();
            task.YamlFileName = Path.Combine(_testFilesPath, @"build2.yml");
            task.Path = "environment.NUGET_API_KEY.secure";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("f9396f803081f4dcafb9eb9fc0e06cd32bb98c2b47185fa8359522b2382c2cea", task.Value);
        }

        [Test(Description = "Read information from YAML file")]
        public void YamlReadExecute_SimpleYamlDocument_NonExistingKey()
        {
            YamlRead task = new YamlRead();
            task.BuildEngine = new MockBuild();
            task.YamlFileName = Path.Combine(_testFilesPath, @"build1.yml");
            task.Path = "non_existing_key";
            Assert.IsFalse(task.Execute(), "Execute Failed");

            Assert.IsNull(task.Value);
        }
    }
}
