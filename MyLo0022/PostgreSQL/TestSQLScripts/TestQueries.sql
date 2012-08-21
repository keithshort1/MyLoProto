select * from activity order by sourceId


select * from location

select * from person

select * from partyactivityparticipation



SELECT L.LocationId FROM Location  AS L  
				WHERE L.street = '' AND L.city = 'New York' AND L.state = 'NY' 
					AND L.zip = '' AND L.country = 'United States' AND L.MyLoAccountId = 1

SELECT DISTINCT country, state, city FROM location ORDER BY country, state, city

select * from activity where sourceid = '10150548827769645'

SELECT DISTINCT locationid, country, state, city FROM location ORDER BY country, state, city

WITH parties AS (
	SELECT PAP.activityid FROM partyactivityparticipation AS PAP
	JOIN person AS P ON P.partyid = PAP.partyid
	WHERE P.name = 'Rebecca Short')
SELECT P.uri FROM Photo AS P
	LEFT JOIN Activity AS A ON P.activityid = A.activityid
	JOIN TimePeriod AS TP ON TP.timeperiodid = A.starttimeperiodid
	LEFT JOIN Location AS L ON L.locationid = A.locationid
	JOIN parties AS PTY ON PTY.activityid = A.activityid
	WHERE P.MyLoAccountId = 1 AND TP.year = 2012
	AND L.country = 'Italy'


WITH parties AS ( 
SELECT PAP.activityid FROM partyactivityparticipation AS PAP 
JOIN person AS P ON P.partyid = PAP.partyid 
WHERE P.name = 'Rebecca Short') 
SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong FROM photo AS P 
LEFT JOIN Activity AS A ON P.activityId = A.activityId 
JOIN TimePeriod AS TP ON A.starttimeperiodid = TP.timeperiodid 
LEFT JOIN Location AS L ON A.locationid = L.locationid 
JOIN parties AS PTY ON PTY.activityid = A.activityid  
WHERE P.MyLoAccountId = 1


SELECT P.uri, P.activityid, timeperiodid FROM Photo AS P
	JOIN Activity AS A ON P.activityid = A.activityid
	LEFT JOIN Location AS L ON L.locationid = A.locationid
	WHERE L.country = 'United States' AND L.city = NULL

select country, city  from location AS L WHERE L.country = 'United States' AND L.city = NULL

select * from location

select * from TimePeriod WHERE year = '2012' AND month=4
SELECT uri, datetaken, camera, gpslat, gpslong FROM photo AS P
                                                            WHERE P.activityid = 25

select to_timestamp(format('01 %s %s', to_char(12::double precision, '99D9'), to_char(2112::double precision, '9999D9')), 'DD MM YYYY')

select format('01 %s %s', select(to_char(12.0::double precision, '99')), select(to_char(2112::double precision, '9999')));
SELECT (format('01 %s %s', '2012', '12'))
select to_char(12.0::double precision, '99')

select * from mylouser

SELECT uri, datetaken, camera, gpslat, gpslong, activityid FROM photo AS P 
JOIN TimePeriod AS TP ON P.timeperiodid = TP.timeperiodid WHERE TP.year = 2012

SELECT uri, datetaken, camera, gpslat, gpslong, thumbnail FROM photo


SELECT P.uri, P.datetaken, P.camera, P.gpslat, P.gpslong FROM photo AS P 
LEFT JOIN Activity AS A ON P.activityId = A.activityId 
LEFT JOIN TimePeriod AS TP ON A.starttimeperiodid = TP.timeperiodid 
LEFT JOIN Location AS L ON A.locationid = L.locationid
WHERE L.locationid = 3 AND TP.timeperiodid = NULL 34



SELECT activityid, locationname, startdatetime, enddatetime, starttimeperiodid, locationid FROM activity ORDER BY startdatetime, locationname

select uniqueid, activityid from photo order by datetaken

select    name, latitude, longitude from geographicalpoint 

select * from photo where uniqueid = '327be0a9-f11e-48d8-a3a9-89685f9571bb'

select uniqueid from photo as P

UPDATE photo_base SET activityId = 38 WHERE uniqueid = 'ef9f8673-3007-442d-a99e-d10517279247' AND myloaccountid = 1

select P.uri, A.locationname, L.city from photo as P 
  left join activity as A ON P.activityid = A.activityId
  JOIN TimePeriod AS T ON A.starttimeperiodid = T.timeperiodid
  LEFT JOIN Location AS L ON A.locationId = L.locationId
where A.myloaccountId = 1 AND L.city = 'Montepulciano'

select * from GeographicalPoint

delete from activity_base where activityid = 55

CREATE OR REPLACE VIEW partyactivityparticipation AS 
 SELECT v.myloaccountid, v.id, v.activityid, v.partyid, v.partytable
   FROM partyactivityparticipation_base v;


alter table partyactivityparticipation_base DROP COLUMN date cascade
delete from person_base where partyid = 6
delete from photo_base