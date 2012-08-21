-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetActivityAndSubActivities(accountId bigint, activityIdIn bigint) CASCADE;

CREATE FUNCTION GetActivityAndSubActivities(accountId bigint, activityIdIn bigint)
	RETURNS TABLE(Id bigint)
	AS $$
 
	WITH RECURSIVE GetActivityAndSubActivities(Id) AS
	( SELECT DISTINCT A.ActivityId as Id 
		FROM Activity AS A
		WHERE A.ActivityId = $2
		AND A.myloaccountId = $1
	  UNION ALL
	  SELECT AH.ChildActivityId 
		FROM GetActivityAndSubActivities AS ASA
		JOIN ActivityHierarchy AS AH ON AH.ParentActivityId = ASA.Id
	)
	SELECT DISTINCT ASA.Id AS Id
	FROM GetActivityAndSubActivities AS ASA
$$ LANGUAGE SQL;


--- TESTS ---------------------
select GetActivityAndSubActivities(1, 566)

select datetaken, uri from photo AS P
	join GetActivityAndSubActivities(1, 563) on P.activityId = GetActivityAndSubActivities