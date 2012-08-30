-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS SetupCursorsActivitiesAndHierarchy(accountId bigint);

CREATE FUNCTION SetupCursorsActivitiesAndHierarchy(accountId bigint)
	RETURNS SETOF refcursor  
	AS $$
	DECLARE
		_hierarchy 	refcursor;
		_activities	refcursor;
	BEGIN
		OPEN _activities FOR EXECUTE
		'SELECT A.ActivityId, A.ActivityKind, A.ActivityName, A.startdatetime, A.enddatetime ' ||
		'FROM Activity AS A ' ||
		'WHERE A.myloaccountId = $1 '
		USING accountId;
		RETURN next _activities;

		OPEN _hierarchy FOR EXECUTE
		'SELECT activityhierachyid, parentactivityid, childactivityid FROM ActivityHierarchy AS AH ' ||
		'WHERE AH.myloaccountid = $1'
		USING accountId;
		RETURN next _hierarchy;

		RETURN;
	END;
$$ LANGUAGE plpgsql;

--- TEST CODE
select (SetupCursorsActivitiesAndHierarchy(1))