-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS AddActivity(accountId bigint, activityKindIn text, sourceIn text, sourceIdIn text, 
	startdatetimeIn timestamp with time zone, enddatetimeIn timestamp with time zone,
	yearIn smallint, monthIn smallint, dayIn text, dayNumberIn smallint, hourIn smallint, 
	yearIn2 smallint, monthIn2 smallint, dayIn2 text, dayNumberIn2 smallint, hourIn2 smallint,
	activityNameIn text, gpsLatIn double precision, gpsLongIn double precision,
	streetIn text, cityIn text, stateIn text, zipIn text, countryIn text, parentActivity bigint);

CREATE FUNCTION AddActivity(accountId bigint, activityKindIn text, sourceIn text, sourceIdIn text, 
	startdatetimeIn timestamp with time zone, enddatetimeIn timestamp with time zone,
	yearIn smallint, monthIn smallint, dayIn text, dayNumberIn smallint, hourIn smallint,
	yearIn2 smallint, monthIn2 smallint, dayIn2 text, dayNumberIn2 smallint, hourIn2 smallint,
	activityNameIn text, gpsLatIn double precision, gpsLongIn double precision,
	streetIn text, cityIn text, stateIn text, zipIn text, countryIn text, parentActivity bigint)
	RETURNS bigint 
	AS $$
	DECLARE
		_id 		bigint;
		_startId	bigint;
		_aId		bigint;
		_endId		bigint;
		_locId		bigint;
		_noAddress	boolean;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'AddActivity - accountId Error';
		ELSEIF activityKindIn IS NULL THEN RAISE EXCEPTION 'AddActivity - activityKind  Error';
		ELSEIF startdatetimeIn IS NULL THEN RAISE EXCEPTION 'AddActivity - date Error';
		ELSE
			_noAddress := (streetIn IS NULL ) AND 
					(cityIn IS NULL ) AND 
					(stateIn IS NULL ) AND 
					(countryIn IS NULL );
			SELECT MyLoAccountId FROM Activity INTO _aId AS A 
				WHERE A.Source = sourceIn AND A.SourceId = sourceIdIn AND A.MyLoAccountId = accountId;
			IF NOT FOUND THEN
				_endId := NULL;
				_startId = (SELECT GetOrInsertTimePeriod(accountId, startdatetimeIn, yearIn, monthIn, dayIn, dayNumberIn, hourIn));
				IF yearIn2 != 0 THEN
					_endId = (SELECT GetOrInsertTimePeriod(accountId, enddatetimeIn, yearIn2, monthIn2, dayIn2, dayNumberIn2, hourIn2));
				END IF;
				IF _noAddress THEN
					_locId := NULL;
				ELSE
					_locId := (SELECT GetOrInsertAddress(accountId, streetIn, cityIn, stateIn, zipIn, countryIn));
				END IF;
				INSERT INTO Activity (MyLoAccountId, ActivityKind, Source, SourceId, 
							ActivityName, StartDateTime, EndDateTime, StartTimePeriodId, EndTimePeriodId, LocationId, Latitude, Longitude)
						VALUES (accountId, activityKindIn, sourceIn, sourceIdIn, 
							activityNameIn, startdatetimeIn, enddatetimeIn, _startId, _endId, _locId, gpsLatIn, gpsLongIn);
				_id = (select currval('ActivitySequence'));

				RETURN _id;	
			END IF;	
		END IF;
		RETURN 1;
	END;
$$ LANGUAGE plpgsql;