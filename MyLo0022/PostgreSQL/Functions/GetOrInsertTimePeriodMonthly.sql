-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetOrInsertTimePeriodMonthly(accountId bigint, yearIn double precision, monthIn double precision, datetime1 timestamp with time zone);

CREATE FUNCTION GetOrInsertTimePeriodMonthly(accountId bigint, yearIn double precision, monthIn double precision, datetime1 timestamp with time zone)
	RETURNS bigint
	AS $$
	DECLARE
		_tpId		bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'GetOrInsertTimePeriod - accountId Error';
		ELSEIF datetime1 IS NULL THEN RAISE EXCEPTION 'GetOrInsertTimePeriod - datetime1 Error %', datetime1;
		ELSE
			SELECT T.TimePeriodId FROM TimePeriod INTO _tpId AS T  
				WHERE T.year = yearIn AND T.month = monthIn AND myloaccountid = accountId;
			IF NOT FOUND THEN
				INSERT INTO TimePeriod(MyLoAccountId, Year, Month, TimePeriodAltKey)
					VALUES (accountId, yearIn, monthIn, datetime1);
				_tpID = (select currval('TimePeriodSequence'));
			END IF;
			return _tpId;	
		END IF;
	END;
$$ LANGUAGE plpgsql;