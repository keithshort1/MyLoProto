-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS SetupIndexCursorsOrdered(accountId bigint);

CREATE FUNCTION SetupIndexCursorsOrdered(accountId bigint)
	RETURNS SETOF refcursor  
	AS $$
	DECLARE
		_photos 	refcursor;
		_activities	refcursor;
	BEGIN
		OPEN _photos FOR EXECUTE
		'SELECT uniqueid, datetaken, gpslat, gpslong, activityid, uri ' ||
		'FROM photo WHERE activityid is NULL AND myloaccountid = $1 ORDER BY datetaken'
		USING accountId;
		RETURN next _photos;

		RETURN;
	END;
$$ LANGUAGE plpgsql;