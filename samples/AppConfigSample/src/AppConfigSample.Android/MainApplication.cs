﻿using System;
using Android.App;
using Android.Runtime;

namespace AppConfigSample.Droid
{
    [Application(
        Label = "Prism Sample",
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