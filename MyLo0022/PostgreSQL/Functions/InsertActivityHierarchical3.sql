-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS InsertActivityHierarchical3(accountId bigint, activityKindIn text, sourceIn text, sourceIdIn text, 
							activityNameIn text, startdatetimeIn timestamp with time zone, 
							enddatetimeIn timestamp with time zone, durationIn int, 
							startId bigint, endId bigint, locId bigint, 
							gpsLatIn double precision, gpsLongIn double precision, 
							parentActivity bigint);

CREATE FUNCTION InsertActivityHierarchical3(accountId bigint, activityKindIn text, sourceIn text, sourceIdIn text, 
							activityNameIn text, startdatetimeIn timestamp with time zone, 
							enddatetimeIn timestamp with time zone, durationIn int, 
							startId bigint, endId bigint, locId bigint, 
							gpsLatIn double precision, gpsLongIn double precision, 
							parentActivity bigint)
	RETURNS bigint
	AS $$
	DECLARE
		_parentId	bigint;
		_childId	bigint;
		_id		bigint;
		_ahId 		bigint;
	BEGIN
		-- Insert the activity ...
		INSERT INTO Activity (MyLoAccountId, ActivityKind, Source, SourceId, 
					ActivityName, StartDateTime, EndDateTime, Duration, StartTimePeriodId, EndTimePeriodId, AddressId, Latitude, Longitude)
				VALUES (accountId, activityKindIn, sourceIn, sourceIdIn, 
					activityNameIn, startdatetimeIn, enddatetimeIn, durationIn, startId, endId, locId, gpsLatIn, gpsLongIn);

		_id = (select currval('ActivitySequence'));
		
		IF parentActivity <> 0 THEN
			-- This case used in the unusual situation of knowing exactly the id of the parent activity
			INSERT INTO ActivityHierarchy(MyLoAccountId, ChildActivityId, ParentActivityId)
						VALUES (accountId, _id, parentActivity);
						
		ELSE				
			-- Find the first, smallest duration activity which can include the input activity
			SELECT A.ActivityId INTO _parentId FROM Activity  AS A  
				WHERE A.MyLoAccountId = accountId 
				AND A.ActivityId <> _id
				AND (A.startdatetime, A.enddatetime) OVERLAPS (startdatetimeIn, enddatetimeIn)
				AND durationIn <= A.Duration AND A.Duration <> 0
				ORDER BY A.Duration
				LIMIT 1;
				
			IF FOUND THEN
				-- Then update any children of the Found activity which may be included in the Input activity
				-- to be children of the Input activity
				UPDATE ActivityHierarchy_base SET ParentActivityId = _id
				WHERE ActivityHierachyId IN  
					(SELECT AH2.ActivityHierachyId FROM ActivityHierarchy AS AH2
						JOIN Activity as A ON AH2.ChildActivityId = A.ActivityId
						WHERE AH2.MyLoAccountId = accountId 
						AND AH2.ParentActivityId = _parentId
						AND (A.startdatetime, A.enddatetime) OVERLAPS (startdatetimeIn, enddatetimeIn)
						AND A.MyLoAccountId = accountId 
						AND A.ActivityId <> _id);
						
				-- Record that the Found activity is the parent of the Input activity
				INSERT INTO ActivityHierarchy(MyLoAccountId, ChildActivityId, ParentActivityId)
							VALUES (accountId, _id, _parentId);
							
				
			ELSE
				-- Did not find an activity to include the Input activity, so it will be a new root.
				-- In which case we must find all the existing root activities that might now be included in it
				-- and record these inclusions in the hierarchy table

				UPDATE ActivityHierarchy_base SET ParentActivityId = _id
				WHERE ActivityHierachyId IN
					(SELECT AH2.ActivityHierachyId FROM ActivityHierarchy AS AH2
						JOIN Activity as A ON AH2.ChildActivityId = A.ActivityId
						WHERE AH2.MyLoAccountId = accountId 
						AND AH2.ParentActivityId IS NULL
						AND (A.startdatetime, A.enddatetime) OVERLAPS (startdatetimeIn, enddatetimeIn)
						AND A.MyLoAccountId = accountId 
						AND A.ActivityId <> _id);
						
				-- and record that the Input activity is a root
				INSERT INTO ActivityHierarchy(MyLoAccountId, ChildActivityId, ParentActivityId)
							VALUES (accountId, _id, NULL);

			END IF;
		END IF;
		RETURN _id;
	END;
$$ LANGUAGE plpgsql;