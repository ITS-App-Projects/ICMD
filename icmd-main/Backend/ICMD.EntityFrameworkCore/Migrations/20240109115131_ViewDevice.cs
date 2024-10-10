using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ViewDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_Device""
 AS
 SELECT ""Device"".""Id"",
    ""DeviceModel"".""Model"",
    ""DeviceModel"".""Description"" AS ""ModelDescription"",
    ""Tag"".""TagName"",
    ""DeviceType"".""Type"",
    ""Process"".""ProcessName"",
    ""ServiceZone"".""Zone"",
    ""ServiceBank"".""Bank"",
    ""Device"".""Service"",
    ""NatureOfSignal"".""NatureOfSignalName"",
    ""SubProcess"".""SubProcessName"",
    ""Stream"".""StreamName"",
    ""EquipmentCode"".""Code"" AS ""EquipmentCode"",
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
    ""Tag"".""SequenceNumber"",
    ""Tag"".""EquipmentIdentifier"",
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
    ""Tag"".""ProjectId""
   FROM ""DeviceModel""
     LEFT JOIN ""Manufacturer"" ON ""DeviceModel"".""ManufacturerId"" = ""Manufacturer"".""Id""
     RIGHT JOIN (""ServiceTrain""
     RIGHT JOIN (""DeviceType""
     RIGHT JOIN (""EquipmentCode""
     RIGHT JOIN (""FailState""
     RIGHT JOIN (""System""
     JOIN ""WorkAreaPack"" ON ""System"".""WorkAreaPackId"" = ""WorkAreaPack"".""Id""
     JOIN ""SubSystem"" ON ""System"".""Id"" = ""SubSystem"".""SystemId""
     RIGHT JOIN (""Device""
     JOIN ""Tag"" ON ""Device"".""TagId"" = ""Tag"".""Id"") ON ""SubSystem"".""Id"" = ""Device"".""SubSystemId"") ON ""FailState"".""Id"" = ""Device"".""FailStateId"") ON ""EquipmentCode"".""Id"" = ""Tag"".""EquipmentCodeId""
     LEFT JOIN ""Stream"" ON ""Tag"".""StreamId"" = ""Stream"".""Id""
     LEFT JOIN ""Process"" ON ""Tag"".""ProcessId"" = ""Process"".""Id""
     LEFT JOIN ""SubProcess"" ON ""Tag"".""SubProcessId"" = ""SubProcess"".""Id""
     LEFT JOIN ""NatureOfSignal"" ON ""Device"".""NatureOfSignalId"" = ""NatureOfSignal"".""Id""
     LEFT JOIN ""ServiceZone"" ON ""Device"".""ServiceZoneId"" = ""ServiceZone"".""Id""
     LEFT JOIN ""ServiceBank"" ON ""Device"".""ServiceBankId"" = ""ServiceBank"".""Id"") ON ""DeviceType"".""Id"" = ""Device"".""DeviceTypeId"") ON ""ServiceTrain"".""Id"" = ""Device"".""ServiceTrainId"") ON ""DeviceModel"".""Id"" = ""Device"".""DeviceModelId"";
");


            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW public.""View_PnIDTagException""
 AS
 SELECT vw.""TagName"",
    vw.""EquipmentCode"",
    vw.""ProcessName"",
    vw.""SubProcessName"",
    vw.""StreamName"",
    vw.""SequenceNumber"",
    vw.""EquipmentIdentifier"",
    vw.""ServiceDescription"",
    skidtag.""TagName"" AS ""SkidTag"",
    ""Tag"".""ProjectId""
   FROM ""Device""
     JOIN ""Tag"" ON ""Tag"".""Id"" = ""Device"".""TagId""
     JOIN ""View_Device"" vw ON vw.""Id"" = ""Device"".""Id""
     LEFT JOIN ""Tag"" skidtag ON skidtag.""Id"" = vw.""SkidTagId""
  WHERE NOT (""Device"".""TagId"" IN ( SELECT ""PnIdTag"".""TagId""
           FROM ""PnIdTag"")) AND (""Device"".""IsInstrument"" = 'Y'::bpchar OR ""Device"".""IsInstrument"" = 'B'::bpchar) AND ""Device"".""IsDeleted"" = false;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
