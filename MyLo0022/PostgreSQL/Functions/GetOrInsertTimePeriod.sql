-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetOrInsertTimePeriod(accountId bigint, datetime1 timestamp with time zone,
						yearIn smallint, monthIn smallint, dayIn text, dayNumberIn smallint, hourIn smallint);

CREATE FUNCTION GetOrInsertTimePeriod(accountId bigint, datetime1 timestamp with time zone, 
						yearIn smallint, monthIn smallint, dayIn text, dayNumberIn smallint, hourIn smallint)
	RETURNS bigint
	AS $$
	DECLARE
		_tpId		bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'GetOrInsertTimePeriod - accountId Error';
		ELSEIF datetime1 IS NULL THEN RAISE EXCEPTION 'GetOrInsertTimePeriod - datetime1 Error';
		ELSE
			SELECT T.TimePeriodId FROM TimePeriod INTO _tpId AS T  
				WHERE T.year = yearIn AND T.month = monthIn AND (T.dayname = dayIn OR T.dayNumber = dayNumberIn)
					AND T.hour = hourIn AND MyloAccountId = accountId;
			IF NOT FOUND THEN
				INSERT INTO TimePeriod(MyLoAccountId, Year, Month, DayName, DayNumber, Hour, TimePeriodAltKey)
					VALUES (accountId, yearIn, monthIn, dayIn, dayNumberIn, hourIn, datetime1);
				_tpID = (select currval('TimePeriodSequence'));
			END IF;
			return _tpId;	
		END IF;
	END;
$$ LANGUAGE plpgsql;