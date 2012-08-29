-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS GetOrInsertGeoLocation(accountId bigint, gpsLatIn double precision, gpsLongIn double precision);

CREATE FUNCTION GetOrInsertGeoLocation(accountId bigint, gpsLatIn double precision, gpsLongIn double precision)
	RETURNS bigint
	AS $$
	DECLARE
		_lId		bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'GetOrInsertGeoLocation - accountId Error';
		ELSEIF gpsLatIn IS NULL THEN RAISE EXCEPTION 'GetOrInsertGeoLocation - no GPS Error';
		ELSE
			SELECT G.LocationId INTO _lId FROM GeoLocation AS G
				WHERE 
					CASE 
						WHEN G.LocationKind = 'Circle' THEN 
							InsideCircle(G.Latitude, G.Longitude, G.Radius, gpsLatIn, gpsLongIn)
						WHEN G.LocationKind = 'Point' THEN
							InsideCircle(G.Latitude, G.Longitude, 0.25, gpsLatIn, gpsLongIn)
					ELSE FALSE
					END
				AND G.MyLoAccountId = accountId
				ORDER BY G.Radius
				LIMIT 1;
			
			IF NOT FOUND THEN
				INSERT INTO GeoLocation(MyLoAccountId, LocationKind, Latitude, Longitude)
					VALUES (accountId, 'Point', gpsLatIn, gpsLongIn);
				_lId = (select currval('GeoLocationSequence'));
			END IF;
			return _lId;	
		END IF;
	END;
$$ LANGUAGE plpgsql;