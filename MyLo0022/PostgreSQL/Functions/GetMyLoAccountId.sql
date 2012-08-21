-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetMyLoAccountId();

CREATE FUNCTION GetMyLoAccountId(uName text) RETURNS bigint 
	AS $$
	DECLARE
		_id 		bigint;
	BEGIN
		_id = (SELECT U.MyLoAccountId FROM MyLoUser AS U WHERE U.UserName = uName);
		RETURN _id;
	END;
$$ LANGUAGE plpgsql;