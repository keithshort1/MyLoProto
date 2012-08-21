-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS SetupIndexCursorsUnOrdered(accountId bigint);

CREATE FUNCTION SetupIndexCursorsUnOrdered(accountId bigint)
	RETURNS SETOF refcursor  
	AS $$
	DECLARE
		_photos 	refcursor;
		_activities	refcursor;
	BEGIN
		OPEN _photos FOR EXECUTE
		'SELECT uniqueid, datetaken, gpslat, gpslong, activityid, uri FROM photo WHERE activityid IS NULL AND myloaccountid = ' || $1;
		RETURN next _photos;

		RETURN;
	END;
$$ LANGUAGE plpgsql;