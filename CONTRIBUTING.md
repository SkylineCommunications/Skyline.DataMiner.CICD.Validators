# How to contribute

## Add error message(s)

Set the `Validator Management Tool` project as startup project. Click on the 'Start' button (Debug > Start Debugging).

In the tool, you can add new error messages via the *Add Check...* button. This will open a new screen which has several fields that need to be filled in. If you are unsure about some of the fields, don't hesitate to contact [Data-Acquisition](mailto:support.data-acquisition@skyline.be) for more information.

- *Category*: Top level tags fall under 'Protocol' and then the lower tags like within the Param tag fall under 'Param'. Similar for the others.
- *Namespace*: This is the 'path' for the tag/attribute. In case there is no check yet for that tag, then you can create a new namespace via the *Add new Namespace...* button.
- *Check Name*: This is the name of the class that will be generated. We use 'CheckABCTag', 'CheckABCAttribute' for checks on specific tags or attributes. In case of a check that involves bigger scenarios, then it just needs to start with 'Check'. QActions have also an exception: When it is a C# analysis check, it starts with 'CSharp'.
- *Error Message Name*: This is the name of the error message. This will become a method when generating the code.
- *Description*: Provide a meaningful description with the necessary placeholders. Similar like `String.Format` you can use `{0}` as placeholders. In case of a more generic error message, you can use the templates. This can be toggled via the checkbox on the right.
- *Description Parameters*: These are the placeholders from the Description.

  - Left column is the argument name if nothing is filled in in the right column.
  - Right column can be prefilled with fixed values (in case when you are using the templates). These will not show up as arguments then.

- *Source*: Either Validator (Validate) or MajorChangeChecker (Compare)
- *Severity:* Specify the severity level. To choose the right severity, you can follow the following guide:

  - Critical: This type of error will have a critical impact on the system or will fully prevent the driver from working. It may also draw your attention to something that needs to be fixed for administrative reasons.
  - Major: This type of error will prevent part of the driver from working as expected. Example: A specific driver feature will not work.
  - Minor: This type of error will not prevent the driver from working, but will have some impact. It may draw your attention to something that was not implemented according to the best practice guidelines. Example: Bad performance, Not user-friendly, etc.
  - Warning: This type of error reveals something that will not have any impact.Example: Unused XML elements or attributes.

- *Certainty*: Specify the certainty level.

  - Certain: An error has been detected and needs to be fixed.
  - Uncertain: A possible error has been detected and needs to be fixed once verified.

- *Fix Impact*: Would there be a breaking change when fixing the error?
- *Has Code Fix*: Is there an automatic fix possible?
- *How To Fix*: Optional field that can describe the steps needed on how to fix the issue.
- *Example Code*: Optional field that can describe how the correct syntax would be.
- *Details*: Optional field that can give extra information regarding the error.

After clicking *Add* it will add the error message to the list. If wanted, you can add other error messages or modify existing ones.

When finished, click *Save* so that the changes are saved to the XML that holds all the error messages. Then click *Generate Code* on the left to start the generation of the C# files. This will generate the Error classes, the check file itself and the unit test files.

## Prepare unit tests

In the `ProtocolTests` project are all the unit tests for the Validator & MajorChangeChecker. You can find your check by following the namespace (Protocol > Params > ...).

Remove all the \[Ignore\] tags and add the necessary error messages. You can have a look at other unit tests how it is done.

In the *Samples* folder you can add or modify the XML files to hold the scenario that should throw the error message.

## Start developing

In the `Protocol` project under the *Tests* folder you can find the same structure as ProtocolTests.

Uncomment the attribute and comment out the interfaces and methods that aren't needed:

- IValidate - Validate(): Validator check
- ICodeFix - Fix(): If 'Has Code Fix' is true
- ICompare - Compare(): Major Change check

## Create pull request

When finished, create a pull request so that we can review and merge your code.

## Contributing to the validator documentation

To contribute to the validator docs, navigate to the Documentation folder and update the markdown file of the check.
Note that you can provide links to the official DataMiner docs repo by using xref links as the xref folder contains a the xref map of DataMiner docs.
For example if you want to add a link to e.g a tag in the Protocol schema, use the following xref instead of an http link:
`[Params tag](xref:Protocol.Params)`.
