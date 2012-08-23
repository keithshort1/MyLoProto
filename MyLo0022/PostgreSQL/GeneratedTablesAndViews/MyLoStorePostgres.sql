-- Postgres 9.0.4 Database for MyLo Store
-- MyLo Inc. Confidential - All Rights Reserved
-- Schema Generated on 8/23/2012 1:43:00 PM

BEGIN; 

--
-- Table Structure for Folder
--
  
DROP TABLE IF EXISTS Folder_Base CASCADE;
  
CREATE TABLE Folder_Base(
    FolderRole varchar(24)  ,
    FolderId bigint not null ,
    FolderName varchar(256) not null ,
    MyLoAccountId bigInt not null,
    Primary Key (FolderId)
);


--
-- Modifiable View Structure for Folder
--
  
DROP SEQUENCE IF EXISTS FolderSequence;
  
CREATE SEQUENCE FolderSequence;
  
DROP VIEW IF EXISTS Folder CASCADE;
  
CREATE VIEW Folder AS
        SELECT  v.MyLoAccountId, v.FolderRole, v.FolderId, v.FolderName
                FROM Folder_Base AS v;

CREATE OR REPLACE FUNCTION FolderInsteadOfInsertProc(NEW Folder) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _FolderId          bigint;
              _mId          bigint;
      BEGIN
              _FolderId := nextval('FolderSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Folder_Base (MyLoAccountId, FolderId, FolderRole, FolderName)
                              VALUES (NEW.MyLoAccountId, _FolderId, NEW.FolderRole, NEW.FolderName);
              END IF;
              RETURN _FolderId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE FolderInsteadOfInsert AS ON INSERT TO Folder
      DO INSTEAD
      SELECT FolderInsteadOfInsertProc(NEW);


--
-- Table Structure for Person
--
  
DROP TABLE IF EXISTS Person_Base CASCADE;
  
CREATE TABLE Person_Base(
    PartyId bigint not null ,
    Name varchar(256) not null ,
    MyLoAccountId bigInt not null,
    Primary Key (PartyId)
);


--
-- Modifiable View Structure for Person
--
  
DROP SEQUENCE IF EXISTS PersonSequence;
  
CREATE SEQUENCE PersonSequence;
  
DROP VIEW IF EXISTS Person CASCADE;
  
CREATE VIEW Person AS
        SELECT  v.MyLoAccountId, v.PartyId, v.Name
                FROM Person_Base AS v;

CREATE OR REPLACE FUNCTION PersonInsteadOfInsertProc(NEW Person) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _PartyId          bigint;
              _mId          bigint;
      BEGIN
              _PartyId := nextval('PersonSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Person_Base (MyLoAccountId, PartyId, Name)
                              VALUES (NEW.MyLoAccountId, _PartyId, NEW.Name);
              END IF;
              RETURN _PartyId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE PersonInsteadOfInsert AS ON INSERT TO Person
      DO INSTEAD
      SELECT PersonInsteadOfInsertProc(NEW);


--
-- Table Structure for PartyGroup
--
  
DROP TABLE IF EXISTS PartyGroup_Base CASCADE;
  
CREATE TABLE PartyGroup_Base(
    GroupKind varchar(24) not null ,
    PartyId bigint not null ,
    Name varchar(256) not null ,
    MyLoAccountId bigInt not null,
    Primary Key (PartyId)
);


--
-- Modifiable View Structure for PartyGroup
--
  
DROP SEQUENCE IF EXISTS PartyGroupSequence;
  
CREATE SEQUENCE PartyGroupSequence;
  
DROP VIEW IF EXISTS PartyGroup CASCADE;
  
CREATE VIEW PartyGroup AS
        SELECT  v.MyLoAccountId, v.GroupKind, v.PartyId, v.Name
                FROM PartyGroup_Base AS v;

CREATE OR REPLACE FUNCTION PartyGroupInsteadOfInsertProc(NEW PartyGroup) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _PartyId          bigint;
              _mId          bigint;
      BEGIN
              _PartyId := nextval('PartyGroupSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO PartyGroup_Base (MyLoAccountId, PartyId, GroupKind, Name)
                              VALUES (NEW.MyLoAccountId, _PartyId, NEW.GroupKind, NEW.Name);
              END IF;
              RETURN _PartyId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE PartyGroupInsteadOfInsert AS ON INSERT TO PartyGroup
      DO INSTEAD
      SELECT PartyGroupInsteadOfInsertProc(NEW);


--
-- Table Structure for GroupMembership
--
  
DROP TABLE IF EXISTS GroupMembership_Base CASCADE;
  
CREATE TABLE GroupMembership_Base(
    StartDateTime timestamp not null ,
    EndDateTime timestamp  ,
    Id bigint not null ,
    PersonId bigInt not null,
    PartyGroupId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (Id),
    UNIQUE (PersonId, PartyGroupId)
);


--
-- Modifiable View Structure for GroupMembership
--
  
DROP SEQUENCE IF EXISTS GroupMembershipSequence;
  
CREATE SEQUENCE GroupMembershipSequence;
  
DROP VIEW IF EXISTS GroupMembership CASCADE;
  
CREATE VIEW GroupMembership AS
        SELECT  v.MyLoAccountId, v.StartDateTime, v.EndDateTime, v.Id, v.PersonId, v.PartyGroupId
                FROM GroupMembership_Base AS v;

CREATE OR REPLACE FUNCTION GroupMembershipInsteadOfInsertProc(NEW GroupMembership) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _Id          bigint;
              _mId          bigint;
      BEGIN
              _Id := nextval('GroupMembershipSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO GroupMembership_Base (MyLoAccountId, Id, StartDateTime, EndDateTime, PersonId, PartyGroupId)
                              VALUES (NEW.MyLoAccountId, _Id, NEW.StartDateTime, NEW.EndDateTime, NEW.PersonId, NEW.PartyGroupId);
              END IF;
              RETURN _Id;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE GroupMembershipInsteadOfInsert AS ON INSERT TO GroupMembership
      DO INSTEAD
      SELECT GroupMembershipInsteadOfInsertProc(NEW);


--
-- Table Structure for FriendShip
--
  
DROP TABLE IF EXISTS FriendShip_Base CASCADE;
  
CREATE TABLE FriendShip_Base(
    Id bigint not null ,
    EndDateTime timestamp  ,
    StartDateTime timestamp not null ,
    Person2Id bigInt not null,
    Person1Id bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (Id),
    UNIQUE (Person2Id, Person1Id)
);


--
-- Modifiable View Structure for FriendShip
--
  
DROP SEQUENCE IF EXISTS FriendShipSequence;
  
CREATE SEQUENCE FriendShipSequence;
  
DROP VIEW IF EXISTS FriendShip CASCADE;
  
CREATE VIEW FriendShip AS
        SELECT  v.MyLoAccountId, v.Id, v.EndDateTime, v.StartDateTime, v.Person2Id, v.Person1Id
                FROM FriendShip_Base AS v;

CREATE OR REPLACE FUNCTION FriendShipInsteadOfInsertProc(NEW FriendShip) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _Id          bigint;
              _mId          bigint;
      BEGIN
              _Id := nextval('FriendShipSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO FriendShip_Base (MyLoAccountId, Id, EndDateTime, StartDateTime, Person2Id, Person1Id)
                              VALUES (NEW.MyLoAccountId, _Id, NEW.EndDateTime, NEW.StartDateTime, NEW.Person2Id, NEW.Person1Id);
              END IF;
              RETURN _Id;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE FriendShipInsteadOfInsert AS ON INSERT TO FriendShip
      DO INSTEAD
      SELECT FriendShipInsteadOfInsertProc(NEW);


--
-- Table Structure for ContactPoint
--
  
DROP TABLE IF EXISTS ContactPoint_Base CASCADE;
  
CREATE TABLE ContactPoint_Base(
    ContactPointId bigint not null ,
    UsesForCommunicationId bigInt not null,
    UsesForCommunicationTable varchar(56) not null,
    ContactForResourceId bigInt not null,
    ContactForResourceTable varchar(56) not null,
    HasPrimaryEmailId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (ContactPointId),
    UNIQUE (UsesForCommunicationId, UsesForCommunicationTable, HasPrimaryEmailId, ContactForResourceId, ContactForResourceTable)
);

DROP INDEX IF EXISTS UsesForCommunicationAbstractRefIndex;

CREATE INDEX UsesForCommunicationAbstractRefIndex on ContactPoint_Base (UsesForCommunicationId, UsesForCommunicationTable);

DROP INDEX IF EXISTS ContactForResourceAbstractRefIndex;

CREATE INDEX ContactForResourceAbstractRefIndex on ContactPoint_Base (ContactForResourceId, ContactForResourceTable);


--
-- Modifiable View Structure for ContactPoint
--
  
DROP SEQUENCE IF EXISTS ContactPointSequence;
  
CREATE SEQUENCE ContactPointSequence;
  
DROP VIEW IF EXISTS ContactPoint CASCADE;
  
CREATE VIEW ContactPoint AS
        SELECT  v.MyLoAccountId, v.ContactPointId, v.HasPrimaryEmailId, v.UsesForCommunicationId, v.UsesForCommunicationTable, v.ContactForResourceId, v.ContactForResourceTable
                FROM ContactPoint_Base AS v;

CREATE OR REPLACE FUNCTION ContactPointInsteadOfInsertProc(NEW ContactPoint) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _ContactPointId          bigint;
              _mId          bigint;
      BEGIN
              _ContactPointId := nextval('ContactPointSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO ContactPoint_Base (MyLoAccountId, ContactPointId, HasPrimaryEmailId, UsesForCommunicationId, UsesForCommunicationTable, ContactForResourceId, ContactForResourceTable)
                              VALUES (NEW.MyLoAccountId, _ContactPointId, NEW.HasPrimaryEmailId, NEW.UsesForCommunicationId, NEW.UsesForCommunicationTable, NEW.ContactForResourceId, NEW.ContactForResourceTable);
              END IF;
              RETURN _ContactPointId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE ContactPointInsteadOfInsert AS ON INSERT TO ContactPoint
      DO INSTEAD
      SELECT ContactPointInsteadOfInsertProc(NEW);


--
-- Table Structure for TelephoneChannel
--
  
DROP TABLE IF EXISTS TelephoneChannel_Base CASCADE;
  
CREATE TABLE TelephoneChannel_Base(
    TelephoneNumber varchar(24) not null ,
    Extension varchar(24) not null ,
    TelephoneNumberType varchar(24) not null ,
    ChannelId bigint not null ,
    MyLoAccountId bigInt not null,
    Primary Key (ChannelId)
);


--
-- Modifiable View Structure for TelephoneChannel
--
  
DROP SEQUENCE IF EXISTS TelephoneChannelSequence;
  
CREATE SEQUENCE TelephoneChannelSequence;
  
DROP VIEW IF EXISTS TelephoneChannel CASCADE;
  
CREATE VIEW TelephoneChannel AS
        SELECT  v.MyLoAccountId, v.TelephoneNumber, v.Extension, v.TelephoneNumberType, v.ChannelId
                FROM TelephoneChannel_Base AS v;

CREATE OR REPLACE FUNCTION TelephoneChannelInsteadOfInsertProc(NEW TelephoneChannel) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _ChannelId          bigint;
              _mId          bigint;
      BEGIN
              _ChannelId := nextval('TelephoneChannelSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO TelephoneChannel_Base (MyLoAccountId, ChannelId, TelephoneNumber, Extension, TelephoneNumberType)
                              VALUES (NEW.MyLoAccountId, _ChannelId, NEW.TelephoneNumber, NEW.Extension, NEW.TelephoneNumberType);
              END IF;
              RETURN _ChannelId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE TelephoneChannelInsteadOfInsert AS ON INSERT TO TelephoneChannel
      DO INSTEAD
      SELECT TelephoneChannelInsteadOfInsertProc(NEW);


--
-- Table Structure for Presence
--
  
DROP TABLE IF EXISTS Presence_Base CASCADE;
  
CREATE TABLE Presence_Base(
    Id bigint not null ,
    IdentityName varchar(256) not null ,
    LocationDescriptionId bigInt not null,
    LocationDescriptionTable varchar(56) not null,
    PartyId bigInt not null,
    PartyTable varchar(56) not null,
    HasPrimaryHomeEmailId bigInt not null,
    FaxId bigInt not null,
    MobileId bigInt not null,
    HomeId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (Id)
);

DROP INDEX IF EXISTS LocationDescriptionAbstractRefIndex;

CREATE INDEX LocationDescriptionAbstractRefIndex on Presence_Base (LocationDescriptionId, LocationDescriptionTable);

DROP INDEX IF EXISTS PartyAbstractRefIndex;

CREATE INDEX PartyAbstractRefIndex on Presence_Base (PartyId, PartyTable);


--
-- Modifiable View Structure for Presence
--
  
DROP SEQUENCE IF EXISTS PresenceSequence;
  
CREATE SEQUENCE PresenceSequence;
  
DROP VIEW IF EXISTS Presence CASCADE;
  
CREATE VIEW Presence AS
        SELECT  v.MyLoAccountId, v.Id, v.IdentityName, v.HasPrimaryHomeEmailId, v.FaxId, v.MobileId, v.HomeId, v.LocationDescriptionId, v.LocationDescriptionTable, v.PartyId, v.PartyTable
                FROM Presence_Base AS v;

CREATE OR REPLACE FUNCTION PresenceInsteadOfInsertProc(NEW Presence) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _Id          bigint;
              _mId          bigint;
      BEGIN
              _Id := nextval('PresenceSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Presence_Base (MyLoAccountId, Id, IdentityName, HasPrimaryHomeEmailId, FaxId, MobileId, HomeId, LocationDescriptionId, LocationDescriptionTable, PartyId, PartyTable)
                              VALUES (NEW.MyLoAccountId, _Id, NEW.IdentityName, NEW.HasPrimaryHomeEmailId, NEW.FaxId, NEW.MobileId, NEW.HomeId, NEW.LocationDescriptionId, NEW.LocationDescriptionTable, NEW.PartyId, NEW.PartyTable);
              END IF;
              RETURN _Id;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE PresenceInsteadOfInsert AS ON INSERT TO Presence
      DO INSTEAD
      SELECT PresenceInsteadOfInsertProc(NEW);


--
-- Table Structure for EmailChannel
--
  
DROP TABLE IF EXISTS EmailChannel_Base CASCADE;
  
CREATE TABLE EmailChannel_Base(
    EmailAlias varchar(256) not null ,
    IsGroup boolean  ,
    ChannelId bigint not null ,
    MyLoAccountId bigInt not null,
    Primary Key (ChannelId)
);


--
-- Modifiable View Structure for EmailChannel
--
  
DROP SEQUENCE IF EXISTS EmailChannelSequence;
  
CREATE SEQUENCE EmailChannelSequence;
  
DROP VIEW IF EXISTS EmailChannel CASCADE;
  
CREATE VIEW EmailChannel AS
        SELECT  v.MyLoAccountId, v.EmailAlias, v.IsGroup, v.ChannelId
                FROM EmailChannel_Base AS v;

CREATE OR REPLACE FUNCTION EmailChannelInsteadOfInsertProc(NEW EmailChannel) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _ChannelId          bigint;
              _mId          bigint;
      BEGIN
              _ChannelId := nextval('EmailChannelSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO EmailChannel_Base (MyLoAccountId, ChannelId, EmailAlias, IsGroup)
                              VALUES (NEW.MyLoAccountId, _ChannelId, NEW.EmailAlias, NEW.IsGroup);
              END IF;
              RETURN _ChannelId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE EmailChannelInsteadOfInsert AS ON INSERT TO EmailChannel
      DO INSTEAD
      SELECT EmailChannelInsteadOfInsertProc(NEW);


--
-- Table Structure for InternetChannel
--
  
DROP TABLE IF EXISTS InternetChannel_Base CASCADE;
  
CREATE TABLE InternetChannel_Base(
    IPAddress varchar(24)  ,
    URL varchar(256)  ,
    ChannelId bigint not null ,
    MyLoAccountId bigInt not null,
    Primary Key (ChannelId)
);


--
-- Modifiable View Structure for InternetChannel
--
  
DROP SEQUENCE IF EXISTS InternetChannelSequence;
  
CREATE SEQUENCE InternetChannelSequence;
  
DROP VIEW IF EXISTS InternetChannel CASCADE;
  
CREATE VIEW InternetChannel AS
        SELECT  v.MyLoAccountId, v.IPAddress, v.URL, v.ChannelId
                FROM InternetChannel_Base AS v;

CREATE OR REPLACE FUNCTION InternetChannelInsteadOfInsertProc(NEW InternetChannel) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _ChannelId          bigint;
              _mId          bigint;
      BEGIN
              _ChannelId := nextval('InternetChannelSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO InternetChannel_Base (MyLoAccountId, ChannelId, IPAddress, URL)
                              VALUES (NEW.MyLoAccountId, _ChannelId, NEW.IPAddress, NEW.URL);
              END IF;
              RETURN _ChannelId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE InternetChannelInsteadOfInsert AS ON INSERT TO InternetChannel
      DO INSTEAD
      SELECT InternetChannelInsteadOfInsertProc(NEW);


--
-- Table Structure for Address
--
  
DROP TABLE IF EXISTS Address_Base CASCADE;
  
CREATE TABLE Address_Base(
    AddressId bigint not null ,
    City text  ,
    Country text  ,
    State text  ,
    Zip varchar(24)  ,
    UTCOffset smallint  ,
    Street text  ,
    MyLoAccountId bigInt not null,
    Primary Key (AddressId)
);


--
-- Modifiable View Structure for Address
--
  
DROP SEQUENCE IF EXISTS AddressSequence;
  
CREATE SEQUENCE AddressSequence;
  
DROP VIEW IF EXISTS Address CASCADE;
  
CREATE VIEW Address AS
        SELECT  v.MyLoAccountId, v.AddressId, v.City, v.Country, v.State, v.Zip, v.UTCOffset, v.Street
                FROM Address_Base AS v;

CREATE OR REPLACE FUNCTION AddressInsteadOfInsertProc(NEW Address) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _AddressId          bigint;
              _mId          bigint;
      BEGIN
              _AddressId := nextval('AddressSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Address_Base (MyLoAccountId, AddressId, City, Country, State, Zip, UTCOffset, Street)
                              VALUES (NEW.MyLoAccountId, _AddressId, NEW.City, NEW.Country, NEW.State, NEW.Zip, NEW.UTCOffset, NEW.Street);
              END IF;
              RETURN _AddressId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE AddressInsteadOfInsert AS ON INSERT TO Address
      DO INSTEAD
      SELECT AddressInsteadOfInsertProc(NEW);


--
-- Table Structure for Activity
--
  
DROP TABLE IF EXISTS Activity_Base CASCADE;
  
CREATE TABLE Activity_Base(
    ActivityId bigint not null ,
    ActivityKind varchar(24)  ,
    Source varchar(256)  ,
    SourceId text  ,
    StartDateTime timestamp with time zone not null ,
    EndDateTime timestamp with time zone  ,
    Duration decimal(6)  ,
    Latitude double precision  ,
    Longitude double precision  ,
    ActivityName text  ,
    AddressId bigInt,
    EndTimePeriodId bigInt,
    GeoLocationId bigInt,
    StartTimePeriodId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (ActivityId)
);


--
-- Modifiable View Structure for Activity
--
  
DROP SEQUENCE IF EXISTS ActivitySequence;
  
CREATE SEQUENCE ActivitySequence;
  
DROP VIEW IF EXISTS Activity CASCADE;
  
CREATE VIEW Activity AS
        SELECT  v.MyLoAccountId, v.ActivityId, v.ActivityKind, v.Source, v.SourceId, v.StartDateTime, v.EndDateTime, v.Duration, v.Latitude, v.Longitude, v.ActivityName, v.StartTimePeriodId, v.AddressId, v.EndTimePeriodId, v.GeoLocationId
                FROM Activity_Base AS v;

CREATE OR REPLACE FUNCTION ActivityInsteadOfInsertProc(NEW Activity) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _ActivityId          bigint;
              _mId          bigint;
      BEGIN
              _ActivityId := nextval('ActivitySequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Activity_Base (MyLoAccountId, ActivityId, ActivityKind, Source, SourceId, StartDateTime, EndDateTime, Duration, Latitude, Longitude, ActivityName, StartTimePeriodId, AddressId, EndTimePeriodId, GeoLocationId)
                              VALUES (NEW.MyLoAccountId, _ActivityId, NEW.ActivityKind, NEW.Source, NEW.SourceId, NEW.StartDateTime, NEW.EndDateTime, NEW.Duration, NEW.Latitude, NEW.Longitude, NEW.ActivityName, NEW.StartTimePeriodId, NEW.AddressId, NEW.EndTimePeriodId, NEW.GeoLocationId);
              END IF;
              RETURN _ActivityId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE ActivityInsteadOfInsert AS ON INSERT TO Activity
      DO INSTEAD
      SELECT ActivityInsteadOfInsertProc(NEW);


--
-- Table Structure for TimePeriod
--
  
DROP TABLE IF EXISTS TimePeriod_Base CASCADE;
  
CREATE TABLE TimePeriod_Base(
    TimePeriodId bigint not null ,
    TimeZone smallint  ,
    TimePeriodAltKey timestamp with time zone not null ,
    Year smallint  ,
    Month smallint  ,
    DayName text  ,
    DayNumber smallint  ,
    Hour smallint  ,
    MyLoAccountId bigInt not null,
    Primary Key (TimePeriodId)
);


--
-- Modifiable View Structure for TimePeriod
--
  
DROP SEQUENCE IF EXISTS TimePeriodSequence;
  
CREATE SEQUENCE TimePeriodSequence;
  
DROP VIEW IF EXISTS TimePeriod CASCADE;
  
CREATE VIEW TimePeriod AS
        SELECT  v.MyLoAccountId, v.TimePeriodId, v.TimeZone, v.TimePeriodAltKey, v.Year, v.Month, v.DayName, v.DayNumber, v.Hour
                FROM TimePeriod_Base AS v;

CREATE OR REPLACE FUNCTION TimePeriodInsteadOfInsertProc(NEW TimePeriod) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _TimePeriodId          bigint;
              _mId          bigint;
      BEGIN
              _TimePeriodId := nextval('TimePeriodSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO TimePeriod_Base (MyLoAccountId, TimePeriodId, TimeZone, TimePeriodAltKey, Year, Month, DayName, DayNumber, Hour)
                              VALUES (NEW.MyLoAccountId, _TimePeriodId, NEW.TimeZone, NEW.TimePeriodAltKey, NEW.Year, NEW.Month, NEW.DayName, NEW.DayNumber, NEW.Hour);
              END IF;
              RETURN _TimePeriodId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE TimePeriodInsteadOfInsert AS ON INSERT TO TimePeriod
      DO INSTEAD
      SELECT TimePeriodInsteadOfInsertProc(NEW);


--
-- Table Structure for Video
--
  
DROP TABLE IF EXISTS Video_Base CASCADE;
  
CREATE TABLE Video_Base(
    UniqueId bigint not null ,
    Uri varchar(256) not null ,
    Hashcode bigint not null ,
    ActivityId bigInt,
    FolderId bigInt,
    TimePeriodId bigInt,
    MyLoAccountId bigInt not null,
    Primary Key (UniqueId)
);


--
-- Modifiable View Structure for Video
--
  
DROP SEQUENCE IF EXISTS VideoSequence;
  
CREATE SEQUENCE VideoSequence;
  
DROP VIEW IF EXISTS Video CASCADE;
  
CREATE VIEW Video AS
        SELECT  v.MyLoAccountId, v.UniqueId, v.Uri, v.Hashcode, v.ActivityId, v.FolderId, v.TimePeriodId
                FROM Video_Base AS v;

CREATE OR REPLACE FUNCTION VideoInsteadOfInsertProc(NEW Video) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _UniqueId          bigint;
              _mId          bigint;
      BEGIN
              _UniqueId := nextval('VideoSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Video_Base (MyLoAccountId, UniqueId, Uri, Hashcode, ActivityId, FolderId, TimePeriodId)
                              VALUES (NEW.MyLoAccountId, _UniqueId, NEW.Uri, NEW.Hashcode, NEW.ActivityId, NEW.FolderId, NEW.TimePeriodId);
              END IF;
              RETURN _UniqueId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE VideoInsteadOfInsert AS ON INSERT TO Video
      DO INSTEAD
      SELECT VideoInsteadOfInsertProc(NEW);


--
-- Table Structure for ActivityHierarchy
--
  
DROP TABLE IF EXISTS ActivityHierarchy_Base CASCADE;
  
CREATE TABLE ActivityHierarchy_Base(
    ActivityHierachyId bigint not null ,
    ChildActivityId bigInt not null,
    ParentActivityId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (ActivityHierachyId)
);

DROP INDEX IF EXISTS ActivityHierarchyIndex;

CREATE INDEX ActivityHierarchyIndex on ActivityHierarchy_Base (ChildActivityId, ParentActivityId);


--
-- Modifiable View Structure for ActivityHierarchy
--
  
DROP SEQUENCE IF EXISTS ActivityHierarchySequence;
  
CREATE SEQUENCE ActivityHierarchySequence;
  
DROP VIEW IF EXISTS ActivityHierarchy CASCADE;
  
CREATE VIEW ActivityHierarchy AS
        SELECT  v.MyLoAccountId, v.ActivityHierachyId, v.ChildActivityId, v.ParentActivityId
                FROM ActivityHierarchy_Base AS v;

CREATE OR REPLACE FUNCTION ActivityHierarchyInsteadOfInsertProc(NEW ActivityHierarchy) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _ActivityHierachyId          bigint;
              _mId          bigint;
      BEGIN
              _ActivityHierachyId := nextval('ActivityHierarchySequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO ActivityHierarchy_Base (MyLoAccountId, ActivityHierachyId, ChildActivityId, ParentActivityId)
                              VALUES (NEW.MyLoAccountId, _ActivityHierachyId, NEW.ChildActivityId, NEW.ParentActivityId);
              END IF;
              RETURN _ActivityHierachyId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE ActivityHierarchyInsteadOfInsert AS ON INSERT TO ActivityHierarchy
      DO INSTEAD
      SELECT ActivityHierarchyInsteadOfInsertProc(NEW);


--
-- Table Structure for PartyActivityParticipation
--
  
DROP TABLE IF EXISTS PartyActivityParticipation_Base CASCADE;
  
CREATE TABLE PartyActivityParticipation_Base(
    Id bigint not null ,
    PartyId bigInt not null,
    PartyTable varchar(56) not null,
    ActivityId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (Id),
    UNIQUE (PartyId, PartyTable, ActivityId)
);

DROP INDEX IF EXISTS PartyAbstractRefIndex;

CREATE INDEX PartyAbstractRefIndex on PartyActivityParticipation_Base (PartyId, PartyTable);


--
-- Modifiable View Structure for PartyActivityParticipation
--
  
DROP SEQUENCE IF EXISTS PartyActivityParticipationSequence;
  
CREATE SEQUENCE PartyActivityParticipationSequence;
  
DROP VIEW IF EXISTS PartyActivityParticipation CASCADE;
  
CREATE VIEW PartyActivityParticipation AS
        SELECT  v.MyLoAccountId, v.Id, v.ActivityId, v.PartyId, v.PartyTable
                FROM PartyActivityParticipation_Base AS v;

CREATE OR REPLACE FUNCTION PartyActivityParticipationInsteadOfInsertProc(NEW PartyActivityParticipation) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _Id          bigint;
              _mId          bigint;
      BEGIN
              _Id := nextval('PartyActivityParticipationSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO PartyActivityParticipation_Base (MyLoAccountId, Id, ActivityId, PartyId, PartyTable)
                              VALUES (NEW.MyLoAccountId, _Id, NEW.ActivityId, NEW.PartyId, NEW.PartyTable);
              END IF;
              RETURN _Id;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE PartyActivityParticipationInsteadOfInsert AS ON INSERT TO PartyActivityParticipation
      DO INSTEAD
      SELECT PartyActivityParticipationInsteadOfInsertProc(NEW);


--
-- Table Structure for AddressHierarchy
--
  
DROP TABLE IF EXISTS AddressHierarchy_Base CASCADE;
  
CREATE TABLE AddressHierarchy_Base(
    AddressHierarchyId bigint not null ,
    ChildLocId bigInt not null,
    ParentLocId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (AddressHierarchyId)
);

DROP INDEX IF EXISTS AddressHierarchyIndex;

CREATE INDEX AddressHierarchyIndex on AddressHierarchy_Base (ParentLocId, ChildLocId);


--
-- Modifiable View Structure for AddressHierarchy
--
  
DROP SEQUENCE IF EXISTS AddressHierarchySequence;
  
CREATE SEQUENCE AddressHierarchySequence;
  
DROP VIEW IF EXISTS AddressHierarchy CASCADE;
  
CREATE VIEW AddressHierarchy AS
        SELECT  v.MyLoAccountId, v.AddressHierarchyId, v.ChildLocId, v.ParentLocId
                FROM AddressHierarchy_Base AS v;

CREATE OR REPLACE FUNCTION AddressHierarchyInsteadOfInsertProc(NEW AddressHierarchy) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _AddressHierarchyId          bigint;
              _mId          bigint;
      BEGIN
              _AddressHierarchyId := nextval('AddressHierarchySequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO AddressHierarchy_Base (MyLoAccountId, AddressHierarchyId, ChildLocId, ParentLocId)
                              VALUES (NEW.MyLoAccountId, _AddressHierarchyId, NEW.ChildLocId, NEW.ParentLocId);
              END IF;
              RETURN _AddressHierarchyId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE AddressHierarchyInsteadOfInsert AS ON INSERT TO AddressHierarchy
      DO INSTEAD
      SELECT AddressHierarchyInsteadOfInsertProc(NEW);


--
-- Table Structure for GeoLocation
--
  
DROP TABLE IF EXISTS GeoLocation_Base CASCADE;
  
CREATE TABLE GeoLocation_Base(
    LocationId bigint not null ,
    LocationName text  ,
    LocationKind text not null ,
    Latitude real  ,
    Longitude real  ,
    Radius real  ,
    DefinesRegionId bigInt,
    MyLoAccountId bigInt not null,
    Primary Key (LocationId)
);


--
-- Modifiable View Structure for GeoLocation
--
  
DROP SEQUENCE IF EXISTS GeoLocationSequence;
  
CREATE SEQUENCE GeoLocationSequence;
  
DROP VIEW IF EXISTS GeoLocation CASCADE;
  
CREATE VIEW GeoLocation AS
        SELECT  v.MyLoAccountId, v.LocationId, v.LocationName, v.LocationKind, v.Latitude, v.Longitude, v.Radius, v.DefinesRegionId
                FROM GeoLocation_Base AS v;

CREATE OR REPLACE FUNCTION GeoLocationInsteadOfInsertProc(NEW GeoLocation) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _LocationId          bigint;
              _mId          bigint;
      BEGIN
              _LocationId := nextval('GeoLocationSequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO GeoLocation_Base (MyLoAccountId, LocationId, LocationName, LocationKind, Latitude, Longitude, Radius, DefinesRegionId)
                              VALUES (NEW.MyLoAccountId, _LocationId, NEW.LocationName, NEW.LocationKind, NEW.Latitude, NEW.Longitude, NEW.Radius, NEW.DefinesRegionId);
              END IF;
              RETURN _LocationId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE GeoLocationInsteadOfInsert AS ON INSERT TO GeoLocation
      DO INSTEAD
      SELECT GeoLocationInsteadOfInsertProc(NEW);


--
-- Table Structure for GeoLocationHierarchy
--
  
DROP TABLE IF EXISTS GeoLocationHierarchy_Base CASCADE;
  
CREATE TABLE GeoLocationHierarchy_Base(
    GeoLocationHierarchyId bigint not null ,
    ParentGeoLocationId bigInt not null,
    ChildGeoLocationId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (GeoLocationHierarchyId)
);

DROP INDEX IF EXISTS GeoLocationHierarchyIndex;

CREATE INDEX GeoLocationHierarchyIndex on GeoLocationHierarchy_Base (ParentGeoLocationId, ChildGeoLocationId);


--
-- Modifiable View Structure for GeoLocationHierarchy
--
  
DROP SEQUENCE IF EXISTS GeoLocationHierarchySequence;
  
CREATE SEQUENCE GeoLocationHierarchySequence;
  
DROP VIEW IF EXISTS GeoLocationHierarchy CASCADE;
  
CREATE VIEW GeoLocationHierarchy AS
        SELECT  v.MyLoAccountId, v.GeoLocationHierarchyId, v.ParentGeoLocationId, v.ChildGeoLocationId
                FROM GeoLocationHierarchy_Base AS v;

CREATE OR REPLACE FUNCTION GeoLocationHierarchyInsteadOfInsertProc(NEW GeoLocationHierarchy) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _GeoLocationHierarchyId          bigint;
              _mId          bigint;
      BEGIN
              _GeoLocationHierarchyId := nextval('GeoLocationHierarchySequence');
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO GeoLocationHierarchy_Base (MyLoAccountId, GeoLocationHierarchyId, ParentGeoLocationId, ChildGeoLocationId)
                              VALUES (NEW.MyLoAccountId, _GeoLocationHierarchyId, NEW.ParentGeoLocationId, NEW.ChildGeoLocationId);
              END IF;
              RETURN _GeoLocationHierarchyId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE GeoLocationHierarchyInsteadOfInsert AS ON INSERT TO GeoLocationHierarchy
      DO INSTEAD
      SELECT GeoLocationHierarchyInsteadOfInsertProc(NEW);


ALTER TABLE GroupMembership_Base ADD CONSTRAINT GroupMembershipPersonId_PartyId FOREIGN KEY (PersonId) REFERENCES Person_Base (PartyId) On Delete Cascade;
ALTER TABLE GroupMembership_Base ADD CONSTRAINT GroupMembershipPartyGroupId_PartyId FOREIGN KEY (PartyGroupId) REFERENCES PartyGroup_Base (PartyId) On Delete Cascade;
ALTER TABLE FriendShip_Base ADD CONSTRAINT Friend2 FOREIGN KEY (Person2Id) REFERENCES Person_Base (PartyId) On Delete Cascade;
ALTER TABLE FriendShip_Base ADD CONSTRAINT Friend1 FOREIGN KEY (Person1Id) REFERENCES Person_Base (PartyId) On Delete Cascade;
ALTER TABLE ContactPoint_Base ADD CONSTRAINT ContactPointHasPrimaryEmailId_ChannelId FOREIGN KEY (HasPrimaryEmailId) REFERENCES EmailChannel_Base (ChannelId) On Delete Cascade;
ALTER TABLE Presence_Base ADD CONSTRAINT PresenceHasPrimaryHomeEmailId_ChannelId FOREIGN KEY (HasPrimaryHomeEmailId) REFERENCES EmailChannel_Base (ChannelId) On Delete Cascade;
ALTER TABLE Presence_Base ADD CONSTRAINT PresenceFaxId_ChannelId FOREIGN KEY (FaxId) REFERENCES TelephoneChannel_Base (ChannelId) On Delete Cascade;
ALTER TABLE Presence_Base ADD CONSTRAINT PresenceMobileId_ChannelId FOREIGN KEY (MobileId) REFERENCES TelephoneChannel_Base (ChannelId) On Delete Cascade;
ALTER TABLE Presence_Base ADD CONSTRAINT PresenceHomeId_ChannelId FOREIGN KEY (HomeId) REFERENCES TelephoneChannel_Base (ChannelId) On Delete Cascade;
ALTER TABLE Activity_Base ADD CONSTRAINT ActivityStartTimePeriodId_TimePeriodId FOREIGN KEY (StartTimePeriodId) REFERENCES TimePeriod_Base (TimePeriodId) On Delete Cascade;
ALTER TABLE ActivityHierarchy_Base ADD CONSTRAINT ActivityHierarchyChildActivityId_ActivityId FOREIGN KEY (ChildActivityId) REFERENCES Activity_Base (ActivityId) On Delete Cascade;
ALTER TABLE ActivityHierarchy_Base ADD CONSTRAINT ActivityHierarchyParentActivityId_ActivityId FOREIGN KEY (ParentActivityId) REFERENCES Activity_Base (ActivityId) On Delete Cascade;
ALTER TABLE PartyActivityParticipation_Base ADD CONSTRAINT PartyActivityParticipationActivityId_ActivityId FOREIGN KEY (ActivityId) REFERENCES Activity_Base (ActivityId) On Delete Cascade;
ALTER TABLE AddressHierarchy_Base ADD CONSTRAINT AddressHierarchyChildLocId_AddressId FOREIGN KEY (ChildLocId) REFERENCES Address_Base (AddressId) On Delete Cascade;
ALTER TABLE AddressHierarchy_Base ADD CONSTRAINT AddressHierarchyParentLocId_AddressId FOREIGN KEY (ParentLocId) REFERENCES Address_Base (AddressId) On Delete Cascade;
ALTER TABLE GeoLocationHierarchy_Base ADD CONSTRAINT GeoLocationHierarchyParentGeoLocationId_LocationId FOREIGN KEY (ParentGeoLocationId) REFERENCES GeoLocation_Base (LocationId) On Delete Cascade;
ALTER TABLE GeoLocationHierarchy_Base ADD CONSTRAINT GeoLocationHierarchyChildGeoLocationId_LocationId FOREIGN KEY (ChildGeoLocationId) REFERENCES GeoLocation_Base (LocationId) On Delete Cascade;
COMMIT; 
