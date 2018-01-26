using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Instabilities.FileLastWriteTime
{
    [TestFixture]
    public class FileLastWriteTime
    {
        public string GetTempDllName()
        {
            return "SomeCompiledLibrary.dll";
        }

        public string Compile(string code)
        {
            var dllPath = GetTempDllName();
            Compile(dllPath, code);
            return dllPath;
        }

        public void Compile(string assemblyPath, string code)
        {
            var compiler = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] {"System.dll"}, assemblyPath);
            compiler.CompileAssemblyFromSource(parameters, code);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            var dllpath = GetTempDllName();
            File.Delete(dllpath);
        }

        [Test]
        [Repeat(100)]
        public void Compile_ValidCode_AlwaysRecompilesDLL()
        {
            var compiledPath = Compile("");
            var compiledFileTimestamp = File.GetLastWriteTime(compiledPath);

            var recompiledPath = Compile("");
            var recompiledFileTimestamp = File.GetLastWriteTime(recompiledPath);

            Assert.That(compiledFileTimestamp, Is.LessThan(recompiledFileTimestamp));
        }
    }
}
