-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS CalculateDuration(startdatetimeIn timestamp with time zone, enddatetimeIn timestamp with time zone);

CREATE FUNCTION CalculateDuration(startdatetimeIn timestamp with time zone, enddatetimeIn timestamp with time zone)
	RETURNS int
	AS $$
	DECLARE
		_duration 	int;
	BEGIN
		IF enddatetimeIn = '-INFINITY' THEN
			_duration := 0;
		ELSE
			_duration := (select EXTRACT (EPOCH FROM  enddatetimeIn - startdatetimeIn)::int)/60;
		END IF;
		RETURN _duration;
	END;
$$ LANGUAGE plpgsql;


-- Tests
select CalculateDuration('2007-07-28 10:15:00'::timestamp, '2007-07-28 11:15:00'::timestamp)