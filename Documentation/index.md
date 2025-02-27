---
uid: index
---

# Introduction

The [Skyline.DataMiner.CICD.Tools.Validator](https://github.com/SkylineCommunications/Skyline.DataMiner.CICD.Validators/tree/main/Skyline.DataMiner.CICD.Tools.Validator) tool is a dotnet tool that can be used to validate connector solutions.
This validator is also part of [DataMiner Integration Studio](https://docs.dataminer.services/develop/TOOLS/DIS/Introduction.html) (DIS), a Visual Studio extension for developing DataMiner connectors, Automation scripts, etc.

## Validator

The validator performs a number of checks on the protocol. The validator performs checks on both the XML elements and the C# code of QActions.

## Major change Checker

The Major Change Checker compares two protocol versions and indicates whether changes are detected that are considered major.
The Major Change Checker is currently only available in DataMiner Integration Studio.

## Check properties

A check consists of the following properties:

- Full ID: Unique ID of the check.
- Name: This is the name of the check.
- Description: A meaningful description of the check with the necessary placeholders.
- Severity: Specify the severity level. To choose the right severity, you can follow the following guide:
  - Critical: This type of error will have a critical impact on the system or will prevent the driver from working. It may also draw your attention to something that needs to be fixed for administrative reasons.
  - Major: This type of error will prevent part of the driver from working as expected. (For example, A specific driver feature will not work.)
  - Minor: This type of error will not prevent the driver from working, but will have some impact. It may draw your attention to something that was not implemented according to the best practice guidelines. (For example, bad performance, Not user-friendly, etc.)
  - Warning: This type of error reveals something that will not have any impact. (For example, unused XML elements or attributes.)
- Source: Either Validator or MajorChangeChecker
- Certainty: Specifies the certainty level.
  - Certain: An error has been detected and needs to be fixed.
  - Uncertain: A possible error has been detected and needs to be fixed once verified.
- Fix Impact: Indicates whether there would be a breaking change when fixing the error.
- Has Code Fix: Indicates whether this check provides an automatic fix.
- How To Fix: This optional field indicates the steps needed on how to fix the issue.
- Example Code: Optional field that can describe how the correct syntax would be.
- Details: This optional field gives additional information regarding the error.

## Check overview

- [Validator cheks](xref:Validator_1_1_1)
- [Major Change Checker checks](xref:MajorChangeChecker_1_23_10)
