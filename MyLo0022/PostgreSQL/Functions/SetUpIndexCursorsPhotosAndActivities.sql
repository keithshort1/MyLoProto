-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS SetupIndexCursorsPhotosAndActivities(accountId bigint);

CREATE FUNCTION SetupIndexCursorsPhotosAndActivities(accountId bigint)
	RETURNS SETOF refcursor  
	AS $$
	DECLARE
		_photos 	refcursor;
		_activities	refcursor;
	BEGIN
		OPEN _photos FOR EXECUTE
		'SELECT uniqueid, datetaken, gpslat, gpslong, activityid, uri FROM photo WHERE activityid IS NULL AND myloaccountid = $1'
		USING accountId;
		RETURN next _photos;

		OPEN _activities FOR EXECUTE
		'SELECT A.ActivityId, A.ActivityKind, A.ActivityName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, L.Street, L.City, L.State, L.Zip, L.Country ' ||
		'FROM Activity AS A ' ||
		'JOIN TimePeriod AS T ON A.starttimeperiodId = T.timeperiodid ' ||
		'LEFT JOIN Address AS L ON A.addressId = L.addressId ' ||
		'WHERE A.myloaccountId = $1'
		USING accountId;
		RETURN next _activities;

		RETURN;
	END;
$$ LANGUAGE plpgsql;