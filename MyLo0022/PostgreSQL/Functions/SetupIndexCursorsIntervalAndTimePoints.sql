-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS SetupIndexCursorsIntervalAndTimePoints(accountId bigint, dateTakenIn timestamp with time zone, hourDiff int);

CREATE FUNCTION SetupIndexCursorsIntervalAndTimePoints(accountId bigint, dateTakenIn timestamp with time zone, hourDiff int)
	RETURNS SETOF refcursor  
	AS $$
	DECLARE
		_activities 	refcursor;
	BEGIN
		OPEN _activities FOR EXECUTE
		'(SELECT A.ActivityId, A.ActivityKind, A.ActivityName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration ' ||
		'FROM Activity AS A ' ||
		'WHERE A.myloaccountId = $1 ' ||
		'AND A.Duration <> 0 ' ||
		'AND  (A.startdatetime, A.enddatetime) OVERLAPS ($2::timestamp, $2::timestamp) ' ||
		'ORDER BY A.Duration) ' ||
		'UNION '
		'SELECT A.ActivityId, A.ActivityKind, A.ActivityName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration ' ||
		'FROM Activity AS A ' ||
		'WHERE A.myloaccountId = $1 ' ||
		'AND A.Duration = 0 ' ||
		'AND  (A.startdatetime - ''$3 hour''::interval, A.startdatetime + ''$3 hour''::interval) OVERLAPS ($2::timestamp, $2::timestamp) '
		USING accountid, dateTakenIn, hourDiff;
		RETURN next _activities;

		RETURN;
	END;
$$ LANGUAGE plpgsql;