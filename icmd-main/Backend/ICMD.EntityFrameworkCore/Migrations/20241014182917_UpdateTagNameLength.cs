using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTagNameLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"do $$            
  declare view_record record;
begin          

  drop table if exists public.__temp_views;

  --at first create temporary table with all view definitions
  create table public.__temp_views(schemaname text, viewname text, definition text, level float);

  --then insert recursive views with levels
  insert into public.__temp_views(schemaname, viewname, definition, level)

  WITH RECURSIVE viewids AS (
   /* all views that don't depend on other views */
   SELECT t.oid, 1 as level
   FROM pg_class t
      JOIN pg_rewrite AS r ON r.ev_class = t.oid
   WHERE r.rulename = '_RETURN'
     AND t.relkind = 'v'
     AND t.relnamespace NOT IN ('pg_catalog'::regnamespace,
                                'information_schema'::regnamespace,
                                'pg_toast'::regnamespace)
     AND NOT EXISTS (
            /* depends on a view */
            SELECT 1
            FROM pg_depend AS d
               JOIN pg_class AS t2 ON d.refobjid = t2.oid
            WHERE d.objid = r.oid
              AND d.classid = 'pg_rewrite'::regclass
              AND d.refclassid = 'pg_class'::regclass
              AND d.deptype = 'n'
              AND d.refobjsubid <> 0
              AND t2.relkind = 'v'
         )
     AND NOT EXISTS (
            /* depends on an extension */
            SELECT 1
            FROM pg_depend
            WHERE objid = t.oid
              AND classid = 'pg_class'::regclass
              AND refclassid = 'pg_extension'::regclass
              AND deptype = 'e'
         )
  UNION ALL
    /* all views that depend on these views */
    SELECT t.oid, viewids.level + 1
    FROM pg_class AS t
        JOIN pg_rewrite AS r ON r.ev_class = t.oid
        JOIN pg_depend AS d ON d.objid = r.oid
        JOIN viewids ON viewids.oid = d.refobjid
    WHERE t.relkind = 'v'
      AND r.rulename = '_RETURN'
      AND d.classid = 'pg_rewrite'::regclass                            
      AND d.refclassid = 'pg_class'::regclass
      AND d.deptype = 'n'
      AND d.refobjsubid <> 0
  )
  /* order the views by level, eliminating duplicates */
  SELECT 'public', oid::regclass, pg_get_viewdef(oid::regclass), max(level)
  FROM viewids
  GROUP BY oid
  ORDER BY max(level);

  --then drop all views
  FOR view_record IN SELECT * from public.__temp_views
  LOOP
    execute format('DROP VIEW IF EXISTS %s.%s CASCADE;', view_record.schemaname, view_record.viewname);
  END LOOP;

  -- then do your other stuff here
  -- for example i'm changing column that depending by views
  alter table if exists public.""Tag"" alter column ""TagName"" type varchar(50);
  alter table if exists public.""InstrumentListImport"" alter column ""Tag"" type varchar(50);
  alter table if exists public.""OMServiceDescriptionImport"" alter column ""Tag"" type varchar(50);
  alter table if exists public.""UIChangeLog"" alter column ""Tag"" type varchar(50);

  --then recreate all the views recursively
  FOR view_record IN SELECT * from public.__temp_views
  LOOP
    execute format('CREATE OR REPLACE VIEW %s.%s AS %s;', view_record.schemaname, view_record.viewname, view_record.definition);
  END LOOP;

  --finally drop temporary table
  drop table public.__temp_views;

end $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"do $$            
  declare view_record record;
begin          

  drop table if exists public.__temp_views;

  --at first create temporary table with all view definitions
  create table public.__temp_views(schemaname text, viewname text, definition text, level float);

  --then insert recursive views with levels
  insert into public.__temp_views(schemaname, viewname, definition, level)

  WITH RECURSIVE viewids AS (
   /* all views that don't depend on other views */
   SELECT t.oid, 1 as level
   FROM pg_class t
      JOIN pg_rewrite AS r ON r.ev_class = t.oid
   WHERE r.rulename = '_RETURN'
     AND t.relkind = 'v'
     AND t.relnamespace NOT IN ('pg_catalog'::regnamespace,
                                'information_schema'::regnamespace,
                                'pg_toast'::regnamespace)
     AND NOT EXISTS (
            /* depends on a view */
            SELECT 1
            FROM pg_depend AS d
               JOIN pg_class AS t2 ON d.refobjid = t2.oid
            WHERE d.objid = r.oid
              AND d.classid = 'pg_rewrite'::regclass
              AND d.refclassid = 'pg_class'::regclass
              AND d.deptype = 'n'
              AND d.refobjsubid <> 0
              AND t2.relkind = 'v'
         )
     AND NOT EXISTS (
            /* depends on an extension */
            SELECT 1
            FROM pg_depend
            WHERE objid = t.oid
              AND classid = 'pg_class'::regclass
              AND refclassid = 'pg_extension'::regclass
              AND deptype = 'e'
         )
  UNION ALL
    /* all views that depend on these views */
    SELECT t.oid, viewids.level + 1
    FROM pg_class AS t
        JOIN pg_rewrite AS r ON r.ev_class = t.oid
        JOIN pg_depend AS d ON d.objid = r.oid
        JOIN viewids ON viewids.oid = d.refobjid
    WHERE t.relkind = 'v'
      AND r.rulename = '_RETURN'
      AND d.classid = 'pg_rewrite'::regclass                            
      AND d.refclassid = 'pg_class'::regclass
      AND d.deptype = 'n'
      AND d.refobjsubid <> 0
  )
  /* order the views by level, eliminating duplicates */
  SELECT 'public', oid::regclass, pg_get_viewdef(oid::regclass), max(level)
  FROM viewids
  GROUP BY oid
  ORDER BY max(level);

  --then drop all views
  FOR view_record IN SELECT * from public.__temp_views
  LOOP
    execute format('DROP VIEW IF EXISTS %s.%s CASCADE;', view_record.schemaname, view_record.viewname);
  END LOOP;

  -- then do your other stuff here
  -- for example i'm changing column that depending by views
  alter table if exists public.""Tag"" alter column ""TagName"" type varchar(25);
  alter table if exists public.""InstrumentListImport"" alter column ""Tag"" type varchar(25);
  alter table if exists public.""OMServiceDescriptionImport"" alter column ""Tag"" type varchar(25);
  alter table if exists public.""UIChangeLog"" alter column ""Tag"" type varchar(25);

  --then recreate all the views recursively
  FOR view_record IN SELECT * from public.__temp_views
  LOOP
    execute format('CREATE OR REPLACE VIEW %s.%s AS %s;', view_record.schemaname, view_record.viewname, view_record.definition);
  END LOOP;

  --finally drop temporary table
  drop table public.__temp_views;

end $$;");
        }
    }
}
