-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS UpdatePhotoActivityId(accountIdIn bigint, uniqueidIn uuid, activityidIn bigint);

CREATE FUNCTION UpdatePhotoActivityId(accountIdIn bigint, uniqueidIn uuid, activityidIn bigint)
	RETURNs int
	AS $$
	BEGIN
		IF accountIdIn IS NULL THEN
			RAISE EXCEPTION 'UPDATEPHOTO MyloAccountId ERROR %', accountIDIn;
		END IF;
		IF uniqueIdIn IS NULL THEN
			RAISE EXCEPTION 'UPDATEPHOTO uniqueId ERROR %', uniqueidIn;
		END IF;
		IF activityidIn IS NULL THEN
			RAISE EXCEPTION 'UPDATEPHOTO activityId ERROR %', activityidIn;
		END IF;
		UPDATE photo_base SET activityId = activityidIn WHERE myloaccountid = accountidIn AND uniqueid = uniqueidIn;
		--RAISE EXCEPTION 'Updated % with %', activityidIn, uniqueidIn;
		RETURN 1;
	END;
$$ LANGUAGE plpgsql;