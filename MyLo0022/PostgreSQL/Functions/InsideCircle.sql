-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS InsideCircle(circleLat double precision, circleLong double precision, radius double precision, 
					gpsLatIn double precision, gpsLongIn double precision);

CREATE FUNCTION InsideCircle(circleLat double precision, circleLong double precision, radius double precision, 
					gpsLatIn double precision, gpsLongIn double precision)
	RETURNS boolean
	AS $$
	DECLARE
		_dlat 		double precision;
		_dlong  	double precision;
		_interimA	double precision;
		_interimB	double precision;
		_distance	double precision;
		_photoLatRad	double precision;
		_photoLongRad	double precision;
		_circleLatRad	double precision;
		_circleLongRad	double precision;
	BEGIN
		-- prototype hack from http://www.ehow.com/facts_6907238_calculate-distance-between-two-lat_longs.html
		-- convert all lat long values to radians
		_photoLatRad := radians(gpsLatIn);
		_photoLongRad := radians(gpsLongIn);
		_circleLatRad := radians(circleLat);
		_circleLongRad := radians(circleLong);

		-- calculate latitude and longitude difference
		_dlat := _photoLatRad - _circleLatRad;
		_dlong := _photoLongRad - _circleLongRad;

		-- interim calculation step 1
		_interimA := power(sin(_dlat / 2), 2) + cos(_photoLatRad) * cos(_circleLatRad) * power(sin(_dlong / 2), 2);

		-- interim calculation step 2
		_interimB := 2 * (atan2(power(_interimA, 0.5), power((1-_interimA), 0.5)));

		-- calculate distance by multiplying radius of Earth in miles
		_distance := _interimB * 3959;

		return abs(_distance) < radius;

	END;
$$ LANGUAGE plpgsql;

