select * from activity

select * from address

select count(*) from photo where activityid is null

select uniqueid, datetaken, activityid , uri from photo order by datetaken

select * from activityhierarchy

select EXTRACT (EPOCH FROM  '2007-07-28 10:15:00'::timestamp - '2007-07-28 09:15:00'::timestamp )::int/60;

SELECT A.ActivityId FROM Activity AS A  
			WHERE A.startdatetime <= '2011-12-07 05:00:00-08'::timestamp AND A.enddatetime > '2011-12-07 14:00:00-08'::timestamp --
			--WHERE (A.enddatetime - A.startdatetime) OVERLAPS (enddatetimeIn - startdatetimeIn)
				AND 540 <= A.Duration
			ORDER BY A.Duration
			LIMIT 1;

(SELECT A.ActivityId, A.ActivityKind, A.LocationName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration
		FROM Activity AS A 
		WHERE A.myloaccountId = 1
		AND  A.startdatetime <= '2012-04-27 09:58:25-07'::timestamp AND A.enddatetime > '2012-04-27 09:58:25-07'::timestamp
		ORDER BY A.Duration)
		UNION ALL
		SELECT A.ActivityId, A.ActivityKind, A.LocationName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration
		FROM Activity AS A 
		WHERE A.myloaccountId = 1
		AND  A.enddatetime = '-infinity' 
		AND A.startdatetime - '4 hour'::interval <= '2012-04-27 09:58:25-07'::timestamp 
		AND '2012-04-27 09:58:25-07'::timestamp < A.startdatetime + '4 hour'::interval;


(SELECT A.ActivityId, A.ActivityKind, A.LocationName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration
		FROM Activity AS A 
		WHERE A.myloaccountId = 1
		AND  A.startdatetime <= '2011-12-07 10:03:42-08'::timestamp AND A.enddatetime > '2011-12-07 10:03:42-08'::timestamp
		ORDER BY A.Duration)
		UNION ALL
		SELECT A.ActivityId, A.ActivityKind, A.LocationName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration
		FROM Activity AS A 
		WHERE A.myloaccountId = 1
		AND  A.enddatetime = '-infinity' 
		AND A.startdatetime - '4 hour'::interval <= '2011-12-07 10:03:42-08'::timestamp 
		AND '2011-12-07 10:03:42-08'::timestamp < A.startdatetime + '4 hour'::interval;

		(SELECT A.ActivityId, A.ActivityKind, A.LocationName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration
		FROM Activity AS A 
		WHERE A.myloaccountId = 1
		AND  A.startdatetime <= '2012-02-23 09:00:53-08'::timestamp AND A.enddatetime > '2012-02-23 09:00:53-08'::timestamp
		ORDER BY A.Duration)
		UNION ALL
		SELECT A.ActivityId, A.ActivityKind, A.LocationName, A.Latitude, A.Longitude, A.startdatetime, A.enddatetime, A.Duration
		FROM Activity AS A 
		WHERE A.myloaccountId = 1
		AND  A.enddatetime = '-infinity' 
		AND A.startdatetime - '4 hour'::interval <= '2012-02-23 09:00:53-08'::timestamp 
		AND '2012-02-23 09:00:53-08'::timestamp < A.startdatetime + '4 hour'::interval;

SELECT A.ActivityId FROM Activity AS A 
			WHERE A.MyLoAccountId = 1 
			AND A.startdatetime >= '2012-06-02 10:00:00-07'::timestamp AND A.enddatetime < '2012-06-02 16:00:00-07'::timestamp
			AND 360 > A.duration
			ORDER BY A.Duration DESC  
			LIMIT 1;

SELECT A.ActivityId FROM Activity AS A 
			WHERE A.MyLoAccountId = 1 AND
			(A.enddatetime <> '-infinity' AND A.startdatetime >= '2012-04-21 12:00:00-07'::timestamp AND A.enddatetime < '2012-04-25 14:00:00-07'::timestamp) 
			OR (A.enddatetime = '-infinity' AND A.startdatetime >= '2012-04-21 12:00:00-07'::timestamp AND A.startdatetime < '2012-04-25 14:00:00-07'::timestamp )
			AND 5880 > A.duration
			ORDER BY A.Duration DESC LIMIT 1

SELECT * FROM Activity AS A
		WHERE ('2012-06-02 10:00:00-07'::timestamp, '2012-06-02 16:00:00-07'::timestamp) 
			OVERLAPS (A.startdatetime, A.enddatetime) 
		AND MyLoAccountId = 1
		ORDER BY A.Duration

SELECT * FROM Activity
WHERE (EXISTS (SELECT AH2.ParentActivityId FROM ActivityHierarchy AS AH2
						WHERE AH2.MyLoAccountId = 1 AND AH2.ParentActivityId = NULL))
				AND (EXISTS (SELECT A.ActivityId FROM Activity as A 
						WHERE(A.startdatetime, A.enddatetime) OVERLAPS ('2012-04-21 12:00:00-07'::timestamp, '2012-04-28 18:00:00-07'::timestamp)
						AND  A.MyLoAccountId = 1 
						AND A.ActivityId <> 86));

UPDATE ActivityHierarchy_base SET ParentActivityId = 196 
WHERE ActivityHierachyId IN (SELECT AH2.ActivityHierachyId FROM ActivityHierarchy AS AH2
			JOIN Activity AS A ON AH2.ChildActivityId = A.ActivityId
			WHERE AH2.MyLoAccountId = 1 
			AND AH2.ParentActivityId IS NULL
			AND (A.startdatetime, A.enddatetime) OVERLAPS ('2012-04-21 12:00:00-07'::timestamp, '2012-04-28 18:00:00-07'::timestamp)
			AND A.MyLoAccountId = 1 
			AND A.ActivityId <> 196)


SELECT A.ActivityId FROM Activity  AS A  
	JOIN Activity as A ON AH2.ChildActivityId = A.ActivityId
	WHERE A.MyLoAccountId = 1 
	AND A.ActivityId <> 426
	AND (A.startdatetime, A.enddatetime) OVERLAPS ('2012-04-27 13:08:14-07'::timestamp, '2012-04-27 13:08:14-07'::timestamp)
	AND 0 <= A.Duration AND A.Duration <> 0
	ORDER BY A.Duration
	LIMIT 1;

SELECT AH2.ActivityHierachyId FROM ActivityHierarchy AS AH2
		WHERE AH2.MyLoAccountId = 1 AND AH2.ParentActivityId = 503;

select * from geolocation

select uri, datetaken, activityid, geolocationid, gpslat, gpslong  from photo

With photies AS ((select uri, datetaken, gpslat, gpslong, A.geolocationid  from photo as P
	join Activity as A on A.activityid = P.activityid
	where A.geolocationid != 0 
	order by A.geolocationid)
union
(select uri, datetaken,  gpslat, gpslong, P.geolocationid  from photo as P
	where P.geolocationid != 0
	order by P.geolocationid)) 
select DISTINCT * from photies order by geolocationid, uri

With photies AS ((select uri, datetaken, gpslat, gpslong, A.geolocationid  from photo as P
	join Activity as A on A.activityid = P.activityid
	where A.geolocationid != 0 
	order by A.geolocationid)
union
(select uri, datetaken,  gpslat, gpslong, P.geolocationid  from photo as P
	where P.geolocationid != 0
	order by P.geolocationid)) 
select DISTINCT PH.geolocationid, count(*) from photies AS PH 
group by geolocationid 

With locations AS (With photies AS ((select uri, datetaken, gpslat, gpslong, A.geolocationid  from photo as P
	join Activity as A on A.activityid = P.activityid
	where A.geolocationid != 0 
	order by A.geolocationid)
union
(select uri, datetaken,  gpslat, gpslong, P.geolocationid  from photo as P
	where P.geolocationid != 0
	order by P.geolocationid)) 
select DISTINCT PH.geolocationid, count(*) from photies AS PH 
group by geolocationid)
select L.latitude, L.longitude, locations.count from locations 
join geolocation as L on L.locationid = locations.geolocationid
order by locations.geolocationid

-- remove the order by clauses
With locations AS 
	(With photies AS ((select uri, datetaken, gpslat, gpslong, A.geolocationid  from photo as P
	join Activity as A on A.activityid = P.activityid
	where A.geolocationid IS NOT NULL AND P.MyloAccountId = 1)
union
(select uri, datetaken,  gpslat, gpslong, P.geolocationid  from photo as P
	where P.geolocationid IS NOT NULL AND P.MyloAccountId = 1)) 
select DISTINCT PH.geolocationid, count(*) from photies AS PH 
group by geolocationid)
select L.latitude, L.longitude, locations.count, locationid from locations 
join geolocation as L on L.locationid = locations.geolocationid
Where l.MyloAccountId = 1
order by locations.geolocationid


SELECT A.activityid, A.activityname, A.startdatetime, A.enddatetime FROM activity AS A
JOIN ActivityHierarchy AS AH ON AH.childactivityid = A.activityid
WHERE A.MyLoAccountId = 1 AND AH.parentactivityid is null
ORDER BY A.startdatetime, A.activityname

SELECT A.activityid, A.activityname, A.startdatetime, A.enddatetime FROM activity AS A
JOIN ActivityHierarchy AS AH ON AH.childactivityid = A.activityid
WHERE A.MyLoAccountId = 1 AND AH.parentactivityid = 29
ORDER BY A.startdatetime, A.activityname

WITH photies AS 
(SELECT p.uri, p.datetaken, p.gpslat, p.gpslong, l.locationid from photo AS P
JOIN Geolocation AS L ON L.LocationId = P.geolocationid
WHERE P.MyLoAccountId = 1 AND L.locationid = 10
UNION
SELECT p.uri, p.datetaken, p.gpslat, p.gpslong, l.locationid FROM photo AS P 
join activity as A on p.activityid = A.activityid
join geolocation AS L on A.geolocationid = L.locationid
WHERE P.MyLoAccountId = 1 AND L.locationid = 10)
SELECT DISTINCT * FROM photies

select * from activityhierarchy where parentactivityid = 29

select * from partyactivityparticipation

select * from photo_base
select * from keywords
select * from geolocation
select * from activity

select * from keywords as K where K.keywords LIKE '%Summer%' AND K.Keywords LIKE '%Cruise%'


delete from activity_base where activitykind = 'Calendar'
delete from activityhierarchy_base
update photo_base SET activityid = NULL
delete from activity_base
delete from photo_base
delete from keywords_base
ALTER TABLE activityHierarchy_base ALTER COLUMN parentactivityId DROP NOT NULL;