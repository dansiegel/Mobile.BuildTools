# Upgrading from 1.X

The Mobile.BuildTools 1.X is extremely reliable for generating app secrets! But 2.0 is much better at it. There were a number of factors that went into determining what we should and should not be doing.

## Secrets

There is a lot about the Secrets class generation that has been completely refactored. The result is that you have far more options when generating secrets than you did in 1.X. The big thing to consider is that you will need to add a configuration for the Project in the solution you want to add secrets for. Secrets have undergone a rename for a variety of reasons. Be sure to configure the `appSettings` section of the `buildtools.json`. For more information on how to configure this be sure to see the [Configuration](../config/appsettings/index.md) documentation.
