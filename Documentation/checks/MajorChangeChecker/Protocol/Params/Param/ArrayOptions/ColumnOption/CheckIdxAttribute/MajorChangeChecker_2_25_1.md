---
uid: MajorChangeChecker_2_25_1
---

# CheckIdxAttribute

## UpdatedIdxValue

<!-- 'Description' and 'Properties' sections are auto-generated. -->
<!-- DON'T TOUCH ME - I'M USED BY VALIDATOR DOC AUTO-GENERATION CODE -->

### Details

The position of a column within SLProtocol is determined by its `idx`, so changing the `idx` of an existing column may have an impact and should therefore be avoided.

This does not apply to a column with `type="displaykey"`, as such a column is not known to SLProtocol. If a `displaykey` column is placed before the last column in a table, the `idx` values of all subsequent columns will no longer correspond to their actual position. We therefore recommend always placing a `displaykey` column as the last column within the `ArrayOptions` tag. Since `displaykey` columns are not known to SLProtocol, the column can safely be shifted back whenever new columns are added, keeping it last without affecting SLProtocol.

<!-- Uncomment to add example code -->
<!--### Example code-->
