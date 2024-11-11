using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.ViewDto
{
    public class ViewNonInstrumentListDto
    {
        public Guid? DeviceId { get; set; }

        [Column("Process No")]
        public string? ProcessNo { get; set; }

        [Column("Sub Process")]
        public string? SubProcess { get; set; }

        public string? StreamName { get; set; }

        [Column("Equipment Code")]
        public string? EquipmentCode { get; set; }

        [Column("Sequence Number")]
        public string? SequenceNumber { get; set; }

        [Column("Equipment Identifier")]
        public string? EquipmentIdentifier { get; set; }

        public string? TagName { get; set; }

        [Column("Device Type")]
        public string? DeviceType { get; set; }

        public char? IsInstrument { get; set; }

        [Column("Connection Parent Tag")]
        public string? ConnectionParentTag { get; set; }

        [Column("Instr Parent Tag")]
        public string? InstrumentParentTag { get; set; }

        [Column("Service Description")]
        public string? ServiceDescription { get; set; }

        public string? Description { get; set; }

        [Column("Nature of Signal")]
        public string? NatureOfSignal { get; set; }

        [Column("DP Node Address")]
        public string? DPNodeAddress { get; set; }

        [Column("No of Slots or Channels")]
        public string? NoOfSlotsChannels { get; set; }

        [Column("Slot Number")]
        public string? SlotNumber { get; set; }

        [Column("PLC Number")]
        public string? PLCNumber { get; set; }

        [Column("PLC Slot Number")]
        public string? PLCSlotNumber { get; set; }

        public string? Location { get; set; }
        public string? Manufacturer { get; set; }

        [Column("Model Number")]
        public string? ModelNumber { get; set; }

        [Column("Model Description")]
        public string? ModelDescription { get; set; }

        [Column("Architecture Drawing")]
        public string? ArchitectureDrawing { get; set; }

        [Column("Architecture Drawing Sheet")]
        public string? ArchitectureDrawingSheet { get; set; }

        public int? Revision { get; set; }

        public string? RevisionChanges { get; set; }

        public Guid? ProjectId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
