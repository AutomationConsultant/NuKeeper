﻿using System.Linq;
using NuGet.Versioning;
using NuKeeper.RepositoryInspection;
using NUnit.Framework;

namespace NuKeeper.Tests.RepositoryInspection
{
    [TestFixture]
    public class PackagesFileReaderTests
    {
        private const string PackagesFileWithPackages = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""foo"" version=""1.2.3.4"" targetFramework=""net45"" />
  <package id=""bar"" version=""2.3.4.5"" targetFramework=""net45"" />
</packages>";

        [Test]
        public void EmptyPackagesListShouldBeParsed()
        {
            const string emptyContents =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
</packages>";

            var packages = PackagesFileReader.Read(emptyContents, TempPath());

            Assert.That(packages, Is.Not.Null);
            Assert.That(packages, Is.Empty);
        }

        [Test]
        public void SinglePackageShouldBeRead()
        {
            const string singlePackage = 
@"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""foo"" version=""1.2.3.4"" targetFramework=""net45"" />
</packages>";

            var packages = PackagesFileReader.Read(singlePackage, TempPath());

            Assert.That(packages, Is.Not.Null);
            Assert.That(packages, Is.Not.Empty);
        }

        [Test]
        public void SinglePackageShouldBePopulated()
        {
            const string singlePackage =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""foo"" version=""1.2.3.4"" targetFramework=""net45"" />
</packages>";

            var packages = PackagesFileReader.Read(singlePackage, TempPath());

            var package = packages.FirstOrDefault();
            PackageAssert.IsPopulated(package);
        }

        [Test]
        public void SinglePackageShouldBeCorrect()
        {
            const string singlePackage =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""foo"" version=""1.2.3.4"" targetFramework=""net45"" />
</packages>";

            var packages = PackagesFileReader.Read(singlePackage, TempPath());

            var package = packages.FirstOrDefault();

            Assert.That(package.Id, Is.EqualTo("foo"));
            Assert.That(package.Version, Is.EqualTo(new NuGetVersion("1.2.3.4")));
            Assert.That(package.Path.PackageReferenceType, Is.EqualTo(PackageReferenceType.PackagesConfig));
        }

        [Test]
        public void TwoPackagesShouldBePopulated()
        {
            var packages = PackagesFileReader.Read(PackagesFileWithPackages, TempPath())
                .ToList();

            Assert.That(packages, Is.Not.Null);
            Assert.That(packages.Count, Is.EqualTo(2));

            PackageAssert.IsPopulated(packages[0]);
            PackageAssert.IsPopulated(packages[1]);
        }

        [Test]
        public void TwoPackagesShouldBeRead()
        {
            var packages = PackagesFileReader.Read(PackagesFileWithPackages, TempPath())
                .ToList();

            Assert.That(packages.Count, Is.EqualTo(2));

            Assert.That(packages[0].Id, Is.EqualTo("foo"));
            Assert.That(packages[0].Version, Is.EqualTo(new NuGetVersion("1.2.3.4")));

            Assert.That(packages[1].Id, Is.EqualTo("bar"));
            Assert.That(packages[1].Version, Is.EqualTo(new NuGetVersion("2.3.4.5")));
        }

        [Test]
        public void ResultIsReiterable()
        {
            var path = TempPath();

            var packages = PackagesFileReader.Read(PackagesFileWithPackages, path);

            foreach (var package in packages)
            {
                PackageAssert.IsPopulated(package);
            }

            Assert.That(packages.Select(p => p.Path), Is.All.EqualTo(path));
        }

        private PackagePath TempPath()
        {
            return new PackagePath("c:\\temp\\somewhere", "src\\packages.config",
                PackageReferenceType.PackagesConfig);
        }
    }
}
