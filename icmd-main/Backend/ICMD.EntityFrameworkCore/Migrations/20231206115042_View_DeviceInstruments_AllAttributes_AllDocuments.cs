using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class View_DeviceInstruments_AllAttributes_AllDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW ""View_Device_Instruments"" AS
            SELECT
                ""Device"".""Id"",
                ""DeviceModel"".""Model"",
                ""DeviceModel"".""Description"" AS ""ModelDescription"",
                ""View_Tag"".""TagName"",
                ""DeviceType"".""Type"",
                ""View_Tag"".""ProcessName"",
                ""ServiceZone"".""Zone"",
                ""ServiceBank"".""Bank"",
                ""Device"".""Service"",
                ""NatureOfSignal"".""NatureOfSignalName"",
                ""View_Tag"".""SubProcessName"",
                ""View_Tag"".""StreamName"",
                ""View_Tag"".""EquipmentCode"",
                ""Device"".""ServiceDescription"",
                ""Device"".""LineVesselNumber"",
                ""Device"".""VendorSupply"",
                ""FailState"".""FailStateName"",
                ""Device"".""Revision"",
                ""Device"".""RevisionChanges"",
                ""Device"".""ModifiedBy"",
                ""Device"".""ModifiedDate"",
                ""Device"".""IsDeleted"",
                ""Manufacturer"".""Name"" AS ""Manufacturer"",
                ""Device"".""Variable"",
                ""ServiceTrain"".""Train"",
                ""Device"".""DeviceModelId"",
                ""View_Tag"".""SequenceNumber"",
                ""View_Tag"".""EquipmentIdentifier"",
                ""Device"".""PanelTagId"",
                ""Device"".""SkidTagId"",
                ""Device"".""StandTagId"",
                ""Device"".""JunctionBoxTagId"",
                ""Device"".""DeviceTypeId"",
                ""Device"".""IsInstrument"",
                ""SubSystem"".""Number"" AS ""SubSystem"",
                ""System"".""Number"" AS ""System"",
                ""WorkAreaPack"".""Number"" AS ""WorkAreaPack"",
                ""Device"".""HistoricalLogging"",
                ""Device"".""HistoricalLoggingFrequency"",
                ""Device"".""HistoricalLoggingResolution"",
                ""View_Tag"".""ProjectId""
                FROM ""DeviceModel""
                JOIN ""Manufacturer"" ON ""DeviceModel"".""ManufacturerId"" = ""Manufacturer"".""Id""
                RIGHT JOIN (
                    ""ServiceTrain""
                    RIGHT JOIN (
                        ""DeviceType""
                        RIGHT JOIN (
                            ""FailState""
                            RIGHT JOIN (
                                ""System""
                                JOIN ""WorkAreaPack"" ON ""System"".""WorkAreaPackId"" = ""WorkAreaPack"".""Id""
                                JOIN ""SubSystem"" ON ""System"".""Id"" = ""SubSystem"".""SystemId""
                                RIGHT JOIN ""Device"" ON ""Device"".""SubSystemId"" = ""SubSystem"".""Id""
                            ) ON ""FailState"".""Id"" = ""Device"".""FailStateId""
                        ) ON ""DeviceType"".""Id"" = ""Device"".""DeviceTypeId""
                    ) ON ""ServiceTrain"".""Id"" = ""Device"".""ServiceTrainId""
                ) ON ""DeviceModel"".""Id"" = ""Device"".""DeviceModelId""
                JOIN ""View_Tag"" ON ""Device"".""TagId"" = ""View_Tag"".""Id""
                LEFT JOIN ""NatureOfSignal"" ON ""Device"".""NatureOfSignalId"" = ""NatureOfSignal"".""Id""
                LEFT JOIN ""ServiceZone"" ON ""Device"".""ServiceZoneId"" = ""ServiceZone"".""Id""
                LEFT JOIN ""ServiceBank"" ON ""Device"".""ServiceBankId"" = ""ServiceBank"".""Id""
                WHERE ""Device"".""IsInstrument"" = 'Y'::bpchar OR ""Device"".""IsInstrument"" = 'B'::bpchar;
            ");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW ""View_AllAttributes"" AS
            SELECT
                cache.""DeviceId"" AS ""Id"",
                def.""Id"" AS ""AttributeDefinitionId"",
                def.""Name"",
                def.""ValueType"",
                val.""Id"" AS ""AttributeValueId"",
                val.""Value""
            FROM ""DeviceAttributeValue"" cache
                JOIN ""AttributeValue"" val ON val.""Id"" = cache.""AttributeValueId""
                JOIN ""AttributeDefinition"" def ON def.""Id"" = val.""AttributeDefinitionId"";
            ");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW ""View_AllDocuments"" AS
            SELECT
                h.""DeviceId"",
                tp.""Type"",
                CASE
                    WHEN ref.""IsVDPDocumentNumber"" = true THEN ((ref.""DocumentNumber""::text || COALESCE('-'::text || ref.""Revision""::text, ''::text)) || COALESCE('-'::text || ref.""Version""::text, ''::text))::character varying
                    ELSE ref.""DocumentNumber""
                END AS ""DocumentNumber"",
                ref.""Sheet""
            FROM ""ReferenceDocument"" ref
                JOIN ""ReferenceDocumentDevice"" h ON h.""ReferenceDocumentId"" = ref.""Id""
                JOIN ""ReferenceDocumentType"" tp ON tp.""Id"" = ref.""ReferenceDocumentTypeId"";
        ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
