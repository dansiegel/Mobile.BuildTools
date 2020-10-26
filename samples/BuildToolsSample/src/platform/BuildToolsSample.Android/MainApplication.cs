using System;
using Android.App;
using Android.Runtime;

namespace BuildToolsSample.Droid
{
    [Application(
        Icon = "@mipmap/icon"
        )]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }
    }
}
