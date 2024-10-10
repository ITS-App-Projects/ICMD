using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMD.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class DefaultMetaDataValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""MetaData"" (""Id"", ""Property"", ""Value"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"") VALUES ('A9CD4538-456D-471B-A917-C005E1404ED5', 'Column Template.CCMD', 'tagName, serviceDescription, manufacturer, modelNumber, pidNumber, natureOfSignal, controlPanelNumber, plcNumber, plcSlotNumber, fieldPanelNumber, dpdpCoupler, dppaCoupler, afdHubNumber, rackNo, slotNo, channelNo, dpNodeAddress, paNodeAddress, instrumentParentTag, workAreaPack, systemCode, subsystemCode', '00000000-0000-0000-0000-000000000000', 'NOW()', 'TRUE', 'FALSE');
INSERT INTO ""MetaData"" (""Id"", ""Property"", ""Value"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"") VALUES ('A7A983EE-6CBE-4AD9-9EA6-EDA38644CD17', 'Column Template.Task Force', 'processNo, subProcess, streamName, equipmentCode, sequenceNumber, equipmentIdentifier, tagName, serviceDescription, manufacturer, modelNumber, calibratedRangeMin, calibratedRangeMax, crUnits, processRangeMin, processRangeMax, prUnits, datasheetNumber, sheetNumber, hookUpDrawing, terminationDiagram, pidNumber, natureOfSignal, gsdType, controlPanelNumber, plcNumber, plcSlotNumber, fieldPanelNumber, dpdpCoupler, dppaCoupler, afdHubNumber, rackNo, slotNo, channelNo, dpNodeAddress, paNodeAddress, revision, revisionChangesOutstandingComments, zone, bank, service, variable, train', '00000000-0000-0000-0000-000000000000', 'NOW()', 'TRUE', 'FALSE');
INSERT INTO ""MetaData"" (""Id"", ""Property"", ""Value"", ""CreatedBy"", ""CreatedDate"", ""IsActive"", ""IsDeleted"") VALUES ('8E31E947-DF4A-4452-B7F3-494D2D92F940', 'Column Template.PSS', 'processNo, subProcess, streamName, equipmentCode, sequenceNumber, equipmentIdentifier, tagName, serviceDescription, area, manufacturer, modelNumber, calibratedRangeMin, calibratedRangeMax, crUnits, processRangeMin, processRangeMax, prUnits, pidNumber, natureOfSignal, failState, gsdType', '00000000-0000-0000-0000-000000000000', 'NOW()', 'TRUE', 'FALSE');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
