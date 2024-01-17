# Skyline.DataMiner.CICD.Validators

## About

### About Skyline.DataMiner.CICD.Validators packages

Skyline.DataMiner.CICD.Validator packages are NuGet packages available in the public [NuGet store](https://www.nuget.org/) that contain assemblies that enhance the CICD experience.

The following packages are available:

- Skyline.DataMiner.CICD.Validators.Common
- Skyline.DataMiner.CICD.Validators.Protocol
- Skyline.DataMiner.CICD.Validators.Protocol.Features

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exist. In addition, you can leverage DataMiner Development Packages to build you own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer)

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

### How to contribute

#### Add error message(s)

Set the `Validator Management Tool` project as startup project. Click on the 'Start' button (Debug > Start Debugging).

In the tool, you can add new error messages via the *Add Check...* button. This will open a new screen which has several fields that need to be filled in. If you are unsure about some of the fields, don't hesitate to contact [Data-Acquisition](mailto:support.data-acquisition@skyline.be) for more information.

- *Category*: Top level tags fall under 'Protocol' and then the lower tags like within the Param tag fall under 'Param'. Similar for the others.
- *Namespace*: This is the 'path' for the tag/attribute. In case there is no check yet for that tag, then you can create a new namespace via the *Add new Namespace...* button.
- *Check Name*: This is the name of the class that will be generated. We use 'CheckABCTag', 'CheckABCAttribute' for checks on specific tags or attributes. In case of a check that involves bigger scenarios, then it just needs to start with 'Check'. QActions have also an exception: When it is a C# analysis check, it starts with 'CSharp'.
- *Error Message Name*: This is the name of the error message. This will become a method when generating the code.
- *Description*: Provide a meaningful description with the necessary placeholders. Similar like `String.Format` you can use `{0}` as placeholders. In case of a more generic error message, you can use the templates. This can be toggled via the checkbox on the right.
- *Description Parameters*: These are the placeholders from the Description.
- *Source*: Either Validator (Validate) or MajorChangeChecker (Compare)
- *Certainty*: How sure are we that the error message is correct?
- *Fix Impact*: Would there be a breaking change when fixing the error?
- *Has Code Fix*: Is there an automatic fix possible?
- *How To Fix*: Optional field that can describe the steps needed on how to fix the issue.
- *Example Code*: Optional field that can describe how the correct syntax would be.
- *Details*: Optional field that can give extra information regarding the error.

After clicking *Add* it will add the error message to the list. If wanted, you can add other error messages or modify existing ones.

When finished, click *Save* so that the changes are saved to the XML that holds all the error messages. Then click *Generate Code* on the left to start the generation of the C# files. This will generate the Error classes, the check file itself and the unit test files.

#### Prepare unit tests

In the `ProtocolTests` project are all the unit tests for the Validator & MajorChangeChecker. You can find your check by following the namespace (Protocol > Params > ...).

Remove all the \[Ignore\] tags and add the necessary error messages. You can have a look at other unit tests how it is done.

In the *Samples* folder you can add or modify the XML files to hold the scenario that should throw the error message.

#### Start developing

In the `Protocol` project under the *Tests* folder you can find the same structure as ProtocolTests.

Uncomment the attribute and comment out the interfaces and methods that aren't needed:

- IValidate - Validate(): Validator check
- ICodeFix - Fix(): If 'Has Code Fix' is true
- ICompare - Compare(): Major Change check

#### Create pull request

When finished, create a pull request so that we can review and merge your code.
