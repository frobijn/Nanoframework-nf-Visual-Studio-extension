// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Targeting.Tooling;
using Targeting.Tooling.Tests.Helpers;

namespace Targeting.Tooling.Tests
{
    [TestClass]
    [TestCategory("Native assemblies")]
    public sealed class AssemblyMetadataTest : TestClassBase
    {
        [TestMethod]
        public void AssemblyMetadata_NanoFrameworkAssembly_Test()
        {
            #region Setup
            string testDirectory = TestDirectoryHelper.GetTestDirectory(TestContext);
            string dllFile = TestDirectoryHelper.CopyEmbeddedResource(GetType(), "mscorlib.dll", testDirectory, "mscorlib.dll");
            string peFile = TestDirectoryHelper.CopyEmbeddedResource(GetType(), "mscorlib.pe", testDirectory, "mscorlib.pe");
            string expectedAssemblyName = "mscorlib";
            string expectedVersion = "1.15.6.0";
            string expectedNativeVersion = "100.5.0.19";
            uint expectedChecksum = 0x445C7AF9;
            #endregion

            #region From .pe file
            var actual = new AssemblyMetadata(peFile);

            Assert.AreEqual(peFile, actual.NanoFrameworkAssemblyFilePath);
            Assert.AreEqual(dllFile, actual.AssemblyFilePath);
            Assert.AreEqual(expectedVersion, actual.Version);
            Assert.AreEqual(expectedAssemblyName, actual.NativeAssembly?.AssemblyName);
            Assert.AreEqual(expectedChecksum, actual.NativeAssembly?.Checksum);
            Assert.AreEqual(expectedNativeVersion, actual.NativeAssembly?.Version);
            #endregion

            #region From .dll file
            actual = new AssemblyMetadata(dllFile);

            Assert.AreEqual(peFile, actual.NanoFrameworkAssemblyFilePath);
            Assert.AreEqual(dllFile, actual.AssemblyFilePath);
            Assert.AreEqual(expectedAssemblyName, actual.NativeAssembly?.AssemblyName);
            Assert.AreEqual(expectedChecksum, actual.NativeAssembly?.Checksum);
            Assert.AreEqual(expectedNativeVersion, actual.NativeAssembly?.Version);
            #endregion
        }

        [TestMethod]
        public void AssemblyMetadata_DotNetAssembly_Test()
        {
            #region Setup
            string testDirectory = TestDirectoryHelper.GetTestDirectory(TestContext);
            string dllFile = TestDirectoryHelper.CopyFile(GetType().Assembly.Location, testDirectory, "library.dll");
            string peFile = TestDirectoryHelper.CopyFile(GetType().Assembly.Location, testDirectory, "library.pe");
            #endregion

            #region From pe file
            var actual = new AssemblyMetadata(peFile);

            Assert.AreEqual(peFile, actual.NanoFrameworkAssemblyFilePath);
            Assert.AreEqual(dllFile, actual.AssemblyFilePath);
            Assert.IsNotNull(actual.Version);
            Assert.IsNull(actual.NativeAssembly);
            #endregion

            #region From .dll file
            actual = new AssemblyMetadata(dllFile);

            Assert.AreEqual(peFile, actual.NanoFrameworkAssemblyFilePath);
            Assert.AreEqual(dllFile, actual.AssemblyFilePath);
            Assert.IsNotNull(actual.Version);
            Assert.IsNull(actual.NativeAssembly);
            #endregion
        }
    }
}
