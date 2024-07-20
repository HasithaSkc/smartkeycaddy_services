--property
ALTER TABLE property 
ADD COLUMN IF NOT EXISTS homebackgroundimageurl character varying(300) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS backgroundimageurl character varying(300) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS qrcode character varying(300) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS messageonreceipt character varying(300) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS welcomemessage character varying(300) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS enablesupportemails boolean;

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS phonenumber character varying(20) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS pinallocationmethod character varying(20) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS keycafestarttime character varying(20) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS Keycafeendtime character varying(20) COLLATE pg_catalog."default";

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS allowcheckinifroomnotready boolean default false;

ALTER TABLE property 
ADD COLUMN IF NOT EXISTS isidscanenabled boolean default true;

--update pin allocation and key cafe start and end times
UPDATE property SET pinallocationmethod = 'KeyCafe' WHERE propertycode = 'arena';
UPDATE property SET keycafestarttime = '11:50PM' WHERE propertycode = 'arena';
UPDATE property SET Keycafeendtime = '06:00AM' WHERE propertycode = 'arena';

--chain
ALTER TABLE chain 
ADD COLUMN IF NOT EXISTS logourl character varying(300) COLLATE pg_catalog."default";

ALTER TABLE chain 
ADD COLUMN IF NOT EXISTS cronumber character varying(20) COLLATE pg_catalog."default";

--update character to character varying
ALTER TABLE property 
ALTER COLUMN propertycode TYPE character varying(20) COLLATE pg_catalog."default";

ALTER TABLE property 
ALTER COLUMN siteinfoqrcodepath TYPE character varying(10) COLLATE pg_catalog."default";

--Delete jsonoutput and back side id image
ALTER TABLE idscanattempt
DROP COLUMN scannedapijsonoutput;

ALTER TABLE idscanattempt
DROP COLUMN backsideimage;