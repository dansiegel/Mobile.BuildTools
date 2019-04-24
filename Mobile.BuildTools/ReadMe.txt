Thanks for installing the Mobile.BuildTools library!

To add build secrets to your application simply add a file named secrets.json to the
root of your project. This will generate a Secrets class in the temporary output 
(obj folder) at build time in the Helpers namespace.

{
  "Foo": "Bar",
  "IsTrue": true,
  "AnInt": 1,
  "ADouble": 1.01
}

namespace YourRootNamespace.Helpers
{
    public static class Secrets
    {
        public const string Foo = "Bar";
        public const bool IsTrue = true;
        public const int AnInt = 1;
        public const double ADouble = 1.01;
    }
}

For more information and configuration settings be sure to check out the Wiki at:
https://github.com/dansiegel/Mobile.BuildTools/wiki