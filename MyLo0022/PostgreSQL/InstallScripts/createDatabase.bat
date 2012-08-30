echo "Creating new MyLo Store database called %1"

cd ..
cd DataBaseSetup

"c:\Program Files\PostgreSQL\9.0\bin\psql" --set tempname=%1 -f 1_MyLoStoreDBCreate.sql -U postgres 


echo "Creating Generated Tables and Views PLDB in database called %1"

cd ..
cd GeneratedTablesAndViews


"c:\Program Files\PostgreSQL\9.0\bin\psql" -f MyLoStorePostgres.sql -U postgres -d %1


echo "Post-Create Actions for MyLo Store database called %1"

cd ..
cd PostCreateScripts

"c:\Program Files\PostgreSQL\9.0\bin\psql" -f 2_InfrastructureTables.sql -U postgres -d %1 


echo "Loading functions into MyLo Store database called %1"

cd ..
cd Functions

"c:\Program Files\PostgreSQL\9.0\bin\psql" -f GetMyLoAccountId.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f InsertActivityHierarchical3.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f AddPhoto.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f AddActivity.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f AddActivityHierarchical.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f SetUpIndexCursorsOrdered.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f SetUpIndexCursorsUnOrdered.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f SetUpIndexCursorsPhotosAndActivities.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f SetUpIndexCursorsIntervalAndTimePoints.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f SetUpCursorsActivitiesAndHierarchy.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f UpdatePhotoActivityId.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f GetActivityAndSubActivities.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f GetOrInsertAddress.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f GetOrInsertTimePeriod.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f GetOrInsertTimePeriodMonthly.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f GetOrInsertGeoLocation.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f AddParty.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f AddPartyToActivityByIds.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f CalculateDuration.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f InsideCircle.sql -U postgres -d %1


echo "Initializing PLDB database called %1" 

cd ..
cd TestSQLScripts

"c:\Program Files\PostgreSQL\9.0\bin\psql" -f InsertMyLoUsers.sql -U postgres -d %1
"c:\Program Files\PostgreSQL\9.0\bin\psql" -f InsertYearMonths.sql -U postgres -d %1

cd ..
cd InstallScripts

echo "All Done"