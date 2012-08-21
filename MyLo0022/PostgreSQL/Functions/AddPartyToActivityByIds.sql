-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS AddPartyToActivityByIds(accountId bigint, partyIdIn bigint, partyKindIn text, activityIdIn bigint);

CREATE FUNCTION AddPartyToActivityByIds(accountId bigint, partyIdIn bigint, partyKindIn text, activityIdIn bigint)
	RETURNS bigint
	AS $$
	DECLARE
		_paId	bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'AddPartyToActivity - accountId Error';
		ELSEIF partyIdIn IS NULL OR partyIdIn = 0 THEN RAISE EXCEPTION 'AddPartyToActivity - partyId  Error';
		ELSEIF activityIdIn IS NULL OR activityIdIn = 0 THEN RAISE EXCEPTION 'AddPartyToActivity - activityId Error';
		ELSE
			_paId = 0;
			SELECT PAP.id FROM PartyActivityParticipation AS PAP INTO _paId 
				WHERE PAP.MyloAccountId = accountid AND PAP.partyid = partyIdIn AND PAP.partytable = partyKindIn
					AND PAP.ActivityId = activityIdIn;
			IF NOT FOUND THEN
				INSERT INTO PartyActivityParticipation (MyLoAccountId, PartyId, PartyTable, ActivityId) 
							VALUES (accountId, partyIdIn, 'Person', activityIdIn);
				_paId = (select currval('partyactivityparticipationSequence'));
			END IF;
			RETURN _paId;
		END IF;
	END;
$$ LANGUAGE plpgsql;