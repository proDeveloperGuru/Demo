-- Table: public.documents
-- DROP TABLE IF EXISTS public.documents;
CREATE TABLE IF NOT EXISTS public.documents (
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY (
        INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1
    ),
    title text COLLATE pg_catalog."default" NOT NULL,
    description text COLLATE pg_catalog."default" NOT NULL,
    responsible_unit text COLLATE pg_catalog."default" NOT NULL,
    created_at timestamp without time zone NOT NULL,
    url text COLLATE pg_catalog."default" NOT NULL,
    file_type text COLLATE pg_catalog."default" NOT NULL,
    estimated_reading_time_minutes integer NOT NULL,
    importance_level text COLLATE pg_catalog."default" NOT NULL,
    category text COLLATE pg_catalog."default" NOT NULL,
    is_active boolean NOT NULL,
    CONSTRAINT documents_pkey PRIMARY KEY (id)
) TABLESPACE pg_default;
ALTER TABLE IF EXISTS public.documents OWNER to admin;