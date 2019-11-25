# F.A.Q.

#### Should every value in my Secrets be "Secret"?

The Secrets Class is a great way to prevent security vulnerbilities created from checking into source control things like Client Id's, Consumer Secrets, or Connection Strings. But it really doesn't have to stop there. In fact the Secrets class is a great way of setting the configuration environment for your application at build. Should your build point to the Dev, Stage, or Production API? This is also a great way of being very intentional about it.

#### Does the Secrets class have to be in the Helpers namespace or named Secrets?

In short no it does not. This has always been configurable though it is much easier with version 2.0 as you can just update the Configuration for the project.

#### Do all of the things that the Mobile.BuildTools is capable of execute every time I run a build?

We try to be smart about what we will and will not do. There is an initialization task that will run on each build which evaluates your project to determine if certain things should or should not occur. For instance if there are no SCSS files in your project that Target will not fire, similarly if you have disabled a target explicitly it should be skipped during the build.
