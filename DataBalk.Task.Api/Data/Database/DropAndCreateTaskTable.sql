-- Script Date: 8/23/2023 3:19 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
DROP TABLE [Task];
CREATE TABLE [Task] (
  [ID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Title] text NOT NULL
, [Description] text NOT NULL
, [Assignee] bigint NOT NULL
, [DueDate] text NULL
);
