-- MyLo Inc. Confidential - All Rights Reserved

DROP FUNCTION IF EXISTS AddParty(accountId bigint, partyNameIn text);

CREATE FUNCTION AddParty(accountId bigint, partyNameIn text)
	RETURNS bigint
	AS $$
	DECLARE
		_pId	bigint;
	BEGIN
		IF accountId IS NULL THEN RAISE EXCEPTION 'AddParty - accountId Error';
		ELSEIF partyNameIn IS NULL THEN RAISE EXCEPTION 'AddParty - partyName Error';
		ELSE
			_pId = 0;
			SELECT P.partyid FROM person AS P INTO _pId WHERE P.name = partyNameIn AND P.MyLoAccountId = accountId;
			IF NOT FOUND THEN
				INSERT INTO person (MyLoAccountId, Name) VALUES (accountId, partyNameIn);
				_pId = (SELECT currval('PersonSequence'));
			END IF;
			RETURN _pId;
		END IF;
	END;
$$ LANGUAGE plpgsql;