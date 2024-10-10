using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.""fn_trAttributeValue_MaintainAttributeLookup_Delete""()
                RETURNS trigger
                LANGUAGE 'plpgsql'
                COST 100
                VOLATILE NOT LEAKPROOF
            AS $BODY$
            BEGIN
	            UPDATE ""MetaData"" SET ""Value"" = 0 WHERE ""Property""='Attribute Cache Valid'; 
            RETURN NULL;
            END; 
            $BODY$;");

            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER trcontrolsystemhierarchy_maintainattributelookup_delete
            AFTER DELETE
            ON public.""ControlSystemHierarchy""
            REFERENCING OLD TABLE AS deleted
            FOR EACH STATEMENT
            EXECUTE FUNCTION public.""fn_trAttributeValue_MaintainAttributeLookup_Delete""();");

            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER trcontrolsystemhierarchy_maintainattributelookup_insert
            AFTER INSERT
            ON public.""ControlSystemHierarchy""
            REFERENCING NEW TABLE AS inserted
            FOR EACH STATEMENT
            EXECUTE FUNCTION public.""fn_trAttributeValue_MaintainAttributeLookup_Delete""();");

            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER trcontrolsystemhierarchy_maintainattributelookup_update
            AFTER UPDATE 
            ON public.""ControlSystemHierarchy""
            REFERENCING NEW TABLE AS inserted OLD TABLE AS deleted
            FOR EACH STATEMENT
            EXECUTE FUNCTION public.""fn_trAttributeValue_MaintainAttributeLookup_Delete""();");

            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER trattributevalue_maintainattributelookup_delete
            AFTER DELETE
            ON public.""AttributeValue""
            REFERENCING OLD TABLE AS deleted
            FOR EACH STATEMENT
            EXECUTE FUNCTION public.""fn_trAttributeValue_MaintainAttributeLookup_Delete""();");

            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER trattributevalue_maintainattributelookup_insert
            AFTER INSERT
            ON public.""AttributeValue""
            REFERENCING NEW TABLE AS inserted
            FOR EACH STATEMENT
            EXECUTE FUNCTION public.""fn_trAttributeValue_MaintainAttributeLookup_Delete""();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
