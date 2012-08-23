-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetOrInsertAddress(accountId bigint, streetIn text, cityIn text, stateIn text, zipIn text, countryIn text);

CREATE FUNCTION GetOrInsertAddress(accountId bigint, streetIn text, cityIn text, stateIn text, zipIn text, countryIn text)
	RETURNS bigint
	AS $$
	DECLARE
		_locId		bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'GetOrInsertAddress - accountId Error';
		ELSEIF (streetIn IS NULL ) AND 
			(cityIn IS NULL ) AND 
			(stateIn IS NULL ) AND 
			(countryIn IS NULL )
			THEN RAISE EXCEPTION 'GetOrInsertAddress - address Error';
		ELSE
			IF countryIn IS NOT NULL  THEN
				SELECT L.AddressId FROM Address INTO _locId AS L 
					WHERE L.country = countryIn AND L.MyLoAccountId = accountId AND 
					L.city IS NULL  AND L.street IS NULL AND L.state IS NULL AND L.zip IS NULL;
				IF NOT FOUND THEN
					INSERT INTO Address(MyLoAccountId, Country) VALUES (accountId, countryIn);
					_locId = (select currval('AddressSequence'));
				END IF;
			END IF;
			IF cityIn IS NOT NULL  AND countryIn IS NOT NULL  THEN
				SELECT L.AddressId FROM Address INTO _locId AS L 
					WHERE L.country = countryIn AND L.MyLoAccountId = accountId AND 
					L.city = cityIn AND L.street IS NULL AND L.state IS NULL ;
				IF NOT FOUND THEN
					INSERT INTO Address(MyLoAccountId, Country, City) VALUES (accountId, countryIn, cityIn);
					_locId = (select currval('AddressSequence'));
				END IF;
			END IF;
			SELECT L.AddressId FROM Address INTO _locId AS L  
				WHERE L.street = streetIn AND L.city = cityIn AND L.state = stateIn 
					--AND L.zip = zipIn AND L.country = countryIn AND L.MyLoAccountId = accountId;
					-- removing need for zipcode test
					AND L.country = countryIn AND L.MyLoAccountId = accountId;
			IF NOT FOUND THEN
				INSERT INTO Address(MyLoAccountId, Street, City, State, Zip, Country)
					VALUES (accountId, streetIn, cityIn, stateIn, zipIn, countryIn);
				_locId = (select currval('AddressSequence'));
			END IF;
			return _locId;	
		END IF;
	END;
$$ LANGUAGE plpgsql