namespace DF.Tests
{
    /// <summary>
    /// Constants to be used in the tests. The constants represent the various classes or members needed to test parts of the functionality
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// An abstract member
        /// </summary>
        public const string AbstractMember = "public abstract void TestMemberName()";

        /// <summary>
        /// A private static class (no members)
        /// </summary>
        public const string EmptyPrivateStaticClass = @"private static class TestClass : ITestClass
                {
                    // Intentionally empty
                }";

        /// <summary>
        /// A public class (no members)
        /// </summary>
        public const string EmptyPublicClass = @"public class TestClass : ITestClass
                {
                    // Intentionally empty
                }";

        /// <summary>
        /// An override member, no body provided.
        /// </summary>
        public const string OverrideMember = "public override void TestMemberName() { }";

        /// <summary>
        /// A private field array member
        /// </summary>
        public const string PrivateFieldArrayMember = "private string[] TestMemberName";

        /// <summary>
        /// A private field list member
        /// </summary>
        public const string PrivateFieldListMember = "private List<string> TestMemberName";

        /// <summary>
        /// A private field member
        /// </summary>
        public const string PrivateFieldMember = "private string TestMemberName";

        /// <summary>
        /// A private method
        /// </summary>
        public const string PrivateMethodMember = @"private TestClass TestMemberName()
        {
            if (instance == null)
            {
                instance = new Singleton();
            }

            return instance;
        }";

        /// <summary>
        /// A public event member
        /// </summary>
        public const string PublicEventMember = "public event DelegateMember TestMemberName;";

        /// <summary>
        /// A public method member, no body provided.
        /// </summary>
        public const string PublicMethodMember = "public void TestMemberName() { }";

        /// <summary>
        /// A public named type member, no body provided.
        /// </summary>
        public const string PublicNamedTypeMember = "private class TestMemberName () { }";

        /// <summary>
        /// A public property member
        /// </summary>
        public const string PublicPropertyMember = "public string TestMemberName { get; set; }";

        /// <summary>
        /// A public static method
        /// </summary>
        public const string PublicStaticMethodMember = @"public static TestClass TestMemberName()
        {
            if (instance == null)
            {
                instance = new Singleton();
            }

            return instance;
        }";

        /// <summary>
        /// A sealed member, no body provided.
        /// </summary>
        public const string SealedMember = "sealed public override void TestMemberName() { }";

        /// <summary>
        /// A static member, no body provided.
        /// </summary>
        public const string StaticMember = "public static void TestMemberName() { }";

        /// <summary>
        /// A virtual member, no body provided.
        /// </summary>
        public const string VirtualMember = "public virtual void TestMemberName() { }";
    }
}