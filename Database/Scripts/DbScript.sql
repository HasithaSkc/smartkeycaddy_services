--device
ALTER TABLE smartkeycaddyuser.keyfobtag 
ADD COLUMN IF NOT EXISTS roomnumber character varying(50) COLLATE pg_catalog."default";
