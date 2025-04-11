---  
uid: Validator_2_39_10  
---

# CheckDisplayKey

## UnexpectedIdxSuffix

### Details

The \[IDX\] suffix on column description is only expected on following columns:  
\- Column included in table with ColumnOption@type\="displaykey".  
\- Column referred by the ArrayOption\/NamingFormat of the containing table only when NamingFormat only consist of that column (no other column, no additional hard\-coded bits).  
\- Column referred by the naming option in the ArrayOption@options attribute of the containing table only when it only consist of that column (no other column).  
\- Column referred by the (obsolete) ArrayOption@displayColumn attribute of the containing table.
