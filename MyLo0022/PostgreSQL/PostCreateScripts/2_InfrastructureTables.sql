--
-- Table Structure for MyLoUser
--
  
DROP TABLE IF EXISTS MyLoUser_Base CASCADE;
  
CREATE TABLE MyLoUser_Base(
    MyLoAccountId bigint not null ,
    UserName varchar(256) not null ,
    Primary Key (MyLoAccountId)
);

CREATE INDEX idx_MyLoUser_Base_Name ON MyLoUser_Base (UserName);

--
-- Modifiable View Structure for MyLoUser
--
  
DROP SEQUENCE IF EXISTS MyLoUserSequence;
  
CREATE SEQUENCE MyLoUserSequence;
  
DROP VIEW IF EXISTS MyLoUser CASCADE;
  
CREATE VIEW MyLoUser AS
        SELECT  v.MyLoAccountId, v.UserName
                FROM MyLoUser_Base AS v;

CREATE OR REPLACE FUNCTION MyLoUserInsteadOfInsertProc(NEW MyLoUser) RETURNS bigint AS $$
      DECLARE
              _error        	boolean;
              _MyLoAccountId    bigint;
      BEGIN
              _MyLoAccountId := nextval('MyLoUserSequence');
              _error := NEW.UserName IN (SELECT UserName FROM MyLoUser_Base WHERE UserName = NEW.UserName);
              IF _error THEN
                      RAISE EXCEPTION 'Duplicate UserName %', NEW.UserName;
              ELSE
                      INSERT INTO MyLoUser_Base (MyLoAccountId, UserName)
                              VALUES (_MyLoAccountId, NEW.UserName);
              END IF;
              RETURN _MyLoAccountId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE MyLoUserInsteadOfInsert AS ON INSERT TO MyLoUser
      DO INSTEAD
      SELECT MyLoUserInsteadOfInsertProc(NEW);


--
-- Table Structure for Preferences
--
  
DROP TABLE IF EXISTS UserProfile_Base CASCADE;
  
CREATE TABLE UserProfile_Base(
    Id bigint not null ,
    Preferences text  ,
    UserId bigInt not null,
    MyLoAccountId bigInt not null,
    EmailId text,
    IsSignedIn boolean,
    Primary Key (Id)
);


--
-- Modifiable View Structure for UserProfile
--
  
DROP SEQUENCE IF EXISTS UserProfileSequence;
  
CREATE SEQUENCE UserProfileSequence;
  
DROP VIEW IF EXISTS UserProfile CASCADE;
  
CREATE VIEW UserProfile AS
        SELECT  v.MyLoAccountId, v.EmailId, v.IsSignedIn, v.Name, v.UserId
                FROM UserProfile_Base AS v;

CREATE OR REPLACE FUNCTION UserProfileInsteadOfInsertProc(NEW UserProfile) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _Id           bigint;
              _mId          bigint;
      BEGIN
              _Id := nextval('UserProfileSequence');
              _mId := (SELECT M.MyLoAcountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId Unknown %', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO UserProfile_Base (MyLoAccountId, Id, EmailId, IsSignedIn, Name, UserId)
                              VALUES (NEW.MyLoAccountId, _Id, NEW.EmailId, NEW.IsSignedIn, NEW.Name, NEW.UserId);
              END IF;
              RETURN _Id;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE UserProfileInsteadOfInsert AS ON INSERT TO UserProfile
      DO INSTEAD
      SELECT UserProfileInsteadOfInsertProc(NEW);



--
-- Table Structure for Preferences
--
  
DROP TABLE IF EXISTS Preferences_Base CASCADE;
  
CREATE TABLE Preferences_Base(
    Id bigint not null ,
    Preferences text  ,
    UserId bigInt not null,
    MyLoAccountId bigInt not null,
    Primary Key (Id)
);


--
-- Modifiable View Structure for Preferences
--
  
DROP SEQUENCE IF EXISTS PreferencesSequence;
  
CREATE SEQUENCE PreferencesSequence;
  
DROP VIEW IF EXISTS Preferences CASCADE;
  
CREATE VIEW Preferences AS
        SELECT  v.MyLoAccountId, v.Preferences, v.UserId
                FROM Preferences_Base AS v;

CREATE OR REPLACE FUNCTION PreferencesInsteadOfInsertProc(NEW Preferences) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _Id           bigint;
              _mId          bigint;
      BEGIN
              _Id := nextval('PreferencesSequence');
              _mId := (SELECT M.MyLoAcountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId Unknown %', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Preferences_Base (MyLoAccountId, Id, Preferences, UserId)
                              VALUES (NEW.MyLoAccountId, _Id, NEW.Preferences, NEW.UserId);
              END IF;
              RETURN _Id;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE PreferencesInsteadOfInsert AS ON INSERT TO Preferences
      DO INSTEAD
      SELECT PreferencesInsteadOfInsertProc(NEW);

--
-- Table Structure for Photo
--
  
DROP TABLE IF EXISTS Photo_Base CASCADE;
  
CREATE TABLE Photo_Base(
    PhotoId bigint NOT NULL ,
    DateTaken timestamp with time zone ,
    Camera varchar(128)  ,
    Thumbnail bytea  ,
    GpsLat double precision  ,
    GpsLong double precision  ,
    UniqueId uuid not null ,
    Uri varchar(256) not null ,
    Hashcode bigint not null ,
    FolderId bigInt ,
    ActivityId bigInt ,
    GeoLocationId bigInt ,
    MyLoAccountId bigInt not null,
    TimePeriodId bigint ,
    Primary Key (PhotoId)
);


--
-- Modifiable View Structure for Photo
--
  
DROP SEQUENCE IF EXISTS PhotoSequence;
  
CREATE SEQUENCE PhotoSequence;
  
DROP VIEW IF EXISTS Photo CASCADE;
  
CREATE VIEW Photo AS
        SELECT  v.MyLoAccountId, v.PhotoId, v.UniqueId, v.DateTaken, v.Camera, v.Thumbnail, v.GpsLat, v.GpsLong, v.Uri, v.Hashcode, v.FolderId, v.ActivityId, v.TimePeriodId, v.GeoLocationId
                FROM Photo_Base AS v;

CREATE OR REPLACE FUNCTION PhotoInsteadOfInsertProc(NEW Photo) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _mId          bigint;
              _pId	    bigint;
      BEGIN
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              _pId := nextval('PhotoSequence');
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Photo_Base (MyLoAccountId, PhotoId, UniqueId, DateTaken, Camera, Thumbnail, GpsLat, GpsLong, Uri, Hashcode, FolderId, ActivityId, TimePeriodId, GeoLocationId)
                              VALUES (NEW.MyLoAccountId, _pId, NEW.UniqueId, NEW.DateTaken, NEW.Camera, NEW.Thumbnail, NEW.GpsLat, NEW.GpsLong, NEW.Uri, NEW.Hashcode, NEW.FolderId, NEW.ActivityId, NEW.TimePeriodId, NEW.GeoLocationId);
              END IF;
              RETURN _pId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE PhotoInsteadOfInsert AS ON INSERT TO Photo
      DO INSTEAD
      SELECT PhotoInsteadOfInsertProc(NEW);


--
-- Table Structure for Keywords
--

DROP SEQUENCE IF EXISTS KeywordsSequence;
  
CREATE SEQUENCE KeywordsSequence;

DROP TABLE IF EXISTS Keywords_base CASCADE;

CREATE TABLE Keywords_base
(
  Keywords character varying(256) NOT NULL,
  Id bigint NOT NULL,
  Keywordsforitemid bigint NOT NULL,
  Keywordsforitemtable character varying(56) NOT NULL,
  MyLoAccountId bigInt not null,
  CONSTRAINT keywords_pkey PRIMARY KEY (Id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE keywords_base OWNER TO postgres;


DROP VIEW IF EXISTS Keywords CASCADE;
  
CREATE VIEW Keywords AS
        SELECT  v.MyLoAccountId, v.Keywords, v.Keywordsforitemid, v.Keywordsforitemtable
                FROM Keywords_Base AS v;
                

CREATE OR REPLACE FUNCTION KeywordsInsteadOfInsertProc(NEW Keywords) RETURNS bigint AS $$
      DECLARE
              _error        boolean;
              _mId          bigint;
              _kId	    bigint;
      BEGIN
              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);
              _kId := nextval('KeywordsSequence');
              IF _mId IS NULL THEN
                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;
              ELSE
                      INSERT INTO Keywords_Base (MyLoAccountId, Id, Keywords, Keywordsforitemid, Keywordsforitemtable)
                              VALUES (NEW.MyLoAccountId, _kId, NEW.Keywords, NEW.Keywordsforitemid, NEW.Keywordsforitemtable);
              END IF;
              RETURN _kId;
      END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE RULE KeywordsInsteadOfInsert AS ON INSERT TO Keywords
      DO INSTEAD
      SELECT KeywordsInsteadOfInsertProc(NEW);

-- Index: keywordsforitemabstractrefindex

DROP INDEX IF EXISTS keywordsforitemabstractrefindex;

CREATE INDEX keywordsforitemabstractrefindex
  ON keywords_base
  USING btree
  (keywordsforitemid, keywordsforitemtable);




--ALTER TABLE Photo_Base ADD CONSTRAINT PhotoFolderId_FolderId FOREIGN KEY (FolderId) REFERENCES Folder_Base (FolderId) On Delete Cascade;
--ALTER TABLE Photo_Base ADD CONSTRAINT PhotoGeoLocationId_GeoLocationId FOREIGN KEY (GeoLocationId) REFERENCES GeoLocation_Base (GeoLocationId) On Delete No Action;
--ALTER TABLE Photo_Base ADD CONSTRAINT PhotoActivityId_ActivityId FOREIGN KEY (ActivityId) REFERENCES Activity_Base (ActivityId) On Delete No Action;
ALTER TABLE UserProfile_Base ADD CONSTRAINT UserProfileUserId_MyLoAccountId FOREIGN KEY (UserId) REFERENCES MyLoUser_Base (MyLoAccountId) On Delete Cascade;
ALTER TABLE UserProfile_Base ADD CONSTRAINT UserProfileMyLoAccountId FOREIGN KEY (MyLoAccountId) REFERENCES MyLoUser_Base (MyLoAccountId) On Delete Cascade;
ALTER TABLE Preferences_Base ADD CONSTRAINT PreferencesUserId_MyLoAccountId FOREIGN KEY (UserId) REFERENCES MyLoUser_Base (MyLoAccountId) On Delete Cascade;
ALTER TABLE Preferences_Base ADD CONSTRAINT PreferencesMyLoAccountId FOREIGN KEY (MyLoAccountId) REFERENCES MyLoUser_Base (MyLoAccountId) On Delete Cascade;
ALTER TABLE ActivityHierarchy_base ALTER COLUMN ParentActivityId DROP NOT NULL;
ALTER TABLE GeoLocationHierarchy_base ALTER COLUMN ParentGeoLocationId DROP NOT NULL;
-- Additional Indexes required on Tables
CREATE INDEX idx_Activities_Source ON Activity_Base (Source, SourceId);
CREATE INDEX idx_TimePeriod_AltKey ON TimePeriod_Base (TimePeriodAltKey);
CREATE INDEX idx_PersonName ON Person_Base (Name);



