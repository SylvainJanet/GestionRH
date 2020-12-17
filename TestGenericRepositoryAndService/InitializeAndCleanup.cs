using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace TestGenericRepositoryAndService
{
    [TestClass]
    public abstract class BaseTest
    {
        #region Properties
        public TestContext TestContext { get; set; }

        public string Class
        {
            get { return this.TestContext.FullyQualifiedTestClassName; }
        }
        public string Method
        {
            get { return this.TestContext.TestName; }
        }
        #endregion

        #region Methods
        protected virtual void Trace(string message)
        {
            Debug.WriteLine(message);

            this.TestContext.WriteLine(message);

            System.Diagnostics.Trace.WriteLine(message);

            //Output.Testing.Trace.WriteLine(message);
        }
        #endregion
    }

    [TestClass]
    public class InitializeAndCleanup
    {

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext TestContext)
        {
            DBTestData.DBTestData.EmptyDb();
            Debug.WriteLine("AssemblyInitialize");
            TestContext.WriteLine("AssemblyInitialize");
            System.Diagnostics.Trace.WriteLine("AssemblyInitialize");
        }

        [AssemblyCleanup]
        public static void AssemblyCleanUp()
        {
            DBTestData.DBTestData.EmptyDb();
            Debug.WriteLine("AssemblyCleanUp");
            System.Diagnostics.Trace.WriteLine("AssemblyCleanUp");
        }
    }
}
