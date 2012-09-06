-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS AddPhoto(accountId bigint, uriIn text, uniqueIdIn uuid, crcIn bigint, dateTakenIn timestamp with time zone, 
				cameraIn text, gpsLatIn double precision, gpsLongIn double precision, 
				thumbnailIn bytea, timeIndexKind text);

CREATE FUNCTION AddPhoto(accountId bigint, uriIn text, uniqueIdIn uuid, crcIn bigint, dateTakenIn timestamp with time zone, 
				cameraIn text, gpsLatIn double precision, gpsLongIn double precision, 
				thumbnailIn bytea, timeIndexKind text)
	RETURNS bigint 
	AS $$
	DECLARE
		_id 		bigint;
		_tId		bigint;
		_lId		bigint;
		_aId		bigint;
		_pId		bigint;
		_year 		double precision;
		_month 		double precision;
		_yearText	text;
		_monthText	text;
		_tempTime	text;
	BEGIN
		IF uriIn IS NULL THEN RAISE EXCEPTION 'AddPhoto - uri Error';
		ELSEIF uniqueIdIn IS NULL THEN RAISE EXCEPTION 'AddPhoto - uniqueId Error';
		ELSEIF dateTakenIn IS NULL THEN RAISE EXCEPTION 'AddPhoto - dateTaken Error';
		ELSE
			IF timeIndexKind = 'Monthly' THEN
				SELECT EXTRACT(YEAR FROM dateTakenIn) INTO _year;
				SELECT EXTRACT(MONTH FROM dateTakenIn) INTO _month;
				_monthText = (SELECT (to_char(_month::double precision, '99')));
				_yearText = (SELECT (to_char(_year::double precision, '9999')));
				_tempTime = '01 ' || _monthText || ' ' || _yearText;
				_tid = (GetOrInsertTimePeriodMonthly(accountId, _year, _month, to_timestamp(_tempTime, 'DD MM YYYY'))); 
			ELSE 
				_tId = NULL;
			END IF;
			IF gpsLatIn <> 0.0 THEN
				_lId = (GetOrInsertGeoLocation(accountId, gpsLatIn, gpsLongIn));
			ELSE
				_lId = NULL;
			END IF;
			_aid = NULL;
			INSERT INTO Photo (MyLoAccountId, UniqueId, Uri, HashCode, ActivityId, TimePeriodId, GeoLocationId, Camera, DateTaken, GpsLat, GpsLong, Thumbnail) 
					VALUES (accountId, uniqueIdIn, uriIn, crcIn, _aId, _tId, _lId, cameraIn, dateTakenIn, gpsLatIn, gpsLongIn, thumbnailIn);
			_pId = (SELECT currval('PhotoSequence'));
			INSERT INTO Keywords(MyLoAccountId, Keywords, Keywordsforitemid, Keywordsforitemtable) 
				VALUES(accountId, uriIn, _pId, 'Photo');
			INSERT INTO Keywords(MyLoAccountId, Keywords, Keywordsforitemid, Keywordsforitemtable) 
				VALUES(accountId, cameraIn, _pId, 'Photo');		
			END IF;		
		RETURN 1;
	END;
$$ LANGUAGE plpgsql;