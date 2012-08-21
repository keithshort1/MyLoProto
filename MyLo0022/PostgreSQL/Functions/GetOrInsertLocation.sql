-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetOrInsertLocation(accountId bigint, streetIn text, cityIn text, stateIn text, zipIn text, countryIn text);

CREATE FUNCTION GetOrInsertLocation(accountId bigint, streetIn text, cityIn text, stateIn text, zipIn text, countryIn text)
	RETURNS bigint
	AS $$
	DECLARE
		_locId		bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'GetOrInsertLocation - accountId Error';
		ELSEIF (streetIn IS NULL ) AND 
			(cityIn IS NULL ) AND 
			(stateIn IS NULL ) AND 
			(countryIn IS NULL )
			THEN RAISE EXCEPTION 'GetOrInsertLocation - address Error';
		ELSE
			IF countryIn IS NOT NULL  THEN
				SELECT L.LocationId FROM Location INTO _locId AS L 
					WHERE L.country = countryIn AND L.MyLoAccountId = accountId AND 
					L.city IS NULL  AND L.street IS NULL AND L.state IS NULL AND L.zip IS NULL;
				IF NOT FOUND THEN
					INSERT INTO Location(MyLoAccountId, Country) VALUES (accountId, countryIn);
					_locId = (select currval('LocationSequence'));
				END IF;
			END IF;
			IF cityIn IS NOT NULL  AND countryIn IS NOT NULL  THEN
				SELECT L.LocationId FROM Location INTO _locId AS L 
					WHERE L.country = countryIn AND L.MyLoAccountId = accountId AND 
					L.city = cityIn AND L.street IS NULL AND L.state IS NULL ;
				IF NOT FOUND THEN
					INSERT INTO Location(MyLoAccountId, Country, City) VALUES (accountId, countryIn, cityIn);
					_locId = (select currval('LocationSequence'));
				END IF;
			END IF;
			SELECT L.LocationId FROM Location INTO _locId AS L  
				WHERE L.street = streetIn AND L.city = cityIn AND L.state = stateIn 
					--AND L.zip = zipIn AND L.country = countryIn AND L.MyLoAccountId = accountId;
					-- removing need for zipcode test
					AND L.country = countryIn AND L.MyLoAccountId = accountId;
			IF NOT FOUND THEN
				INSERT INTO Location(MyLoAccountId, Street, City, State, Zip, Country)
					VALUES (accountId, streetIn, cityIn, stateIn, zipIn, countryIn);
				_locId = (select currval('LocationSequence'));
			END IF;
			return _locId;	
		END IF;
	END;
$$ LANGUAGE plpgsql