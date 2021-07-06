**Q.** I'm using a shared project with my Uno app, each app obviously has a different namespace. How can I get the generated class to use a specific root namespace?
**A.** When configuring the AppSettings there are 2 namespace fields. You will need to set the `rootNamespace`. This will override the RootNamespace of the compiling project.

**Q.** How can change the default namespace of the generated class to be in the root namespace of my project?
**A.** Simply provide a value of `.` for the namespace property

**Q.** What is the `delimiter` used for?
**A.** Json objects are effectively a dictionary which cannot have repeated keys, similarly Environment Variables operate the same way. As a result things which are ultimately lists such as the PATH Environment property utilize a delimiter such as `;` to separate the items in the list. By default the Mobile.BuildTools will use the semi-colon, though you can use any character or string you want. Any variables you've defined as arrays will use that delimiter to split the string into an array of whatever your datatype is.

**Q.** Is there a way to get the generated class to use a List instead of an Array?
**A.** No, this is a limitation of the API. If this is an issue for you please open an issue and explain your use case.
