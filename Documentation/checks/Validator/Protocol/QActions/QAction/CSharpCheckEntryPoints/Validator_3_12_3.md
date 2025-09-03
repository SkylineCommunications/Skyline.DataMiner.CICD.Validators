---
uid: Validator_3_12_3
---

# CSharpCheckEntryPoints

## UnexpectedAccessModifierForEntryPointClass

<!-- Description, Properties, ... sections are auto-generated. -->
<!-- REPLACE ME AUTO-GENERATION -->

### Details

The QAction@entryPoint attribute can optionally be used to overwrite the default entry point of a QAction.

By default, the entry point will be the method 'Run' that can be found under the class 'QAction'.
Defining multiple entry points can be done by providing a semicolon list of entry points.
In that case, each entry point corresponds to a QAction trigger.

One of the two following format can be used in order to overwrite default entry points (curly braces are here used as place holder indicators in the below explanation so are not to be included when defining your entry points):
- {entryPointMethod}: the given method name in the 'QAction' class will be the new entry point.
- {entryPointClass}.{entryPointMethod}: the given method name in the given class name will be the new entry point.

Note that both the entry point class and methods are expected to be public and the first argument of the entry point is expected to be of type SLProtocol or SLProtocolExt.

<!-- Uncomment to add example code -->
<!--### Example code-->
