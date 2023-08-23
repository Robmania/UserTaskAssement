-- Script Date: 8/23/2023 3:20 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
DROP TABLE [User];
CREATE TABLE [User] (
  [ID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Username] text NOT NULL
, [Email] text NOT NULL
, [Password] text NOT NULL
);
