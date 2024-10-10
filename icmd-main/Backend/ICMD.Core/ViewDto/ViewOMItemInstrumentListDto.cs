using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewOMItemInstrumentListDto
    {
        public Guid? Id { get; set; }
        public string? ItemId { get; set; }
        public string? ItemDescription { get; set; }
        public string? ParentItemId { get; set; }
        public string? AssetTypeId { get; set; }

        //public Guid? DeviceId { get; set; }

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
        public string? Tag { get; set; }

        [Column("Instr Parent Tag")]
        public string? InstrumentParentTag { get; set; }

        [Column("Service Description")]
        public string? ServiceDescription { get; set; }

        [Column("Line / Vessel Number")]
        public string? LineVesselNumber { get; set; }

        public int? Plant { get; set; }
        public int? Area { get; set; }

        [Column("Vendor Supply")]
        public bool? VendorSupply { get; set; }

        [Column("Skid Number")]
        public string? SkidNumber { get; set; }

        [Column("Stand Number")]
        public string? StandNumber { get; set; }


        public string? Manufacturer { get; set; }

        [Column("Model Number")]
        public string? ModelNumber { get; set; }

        [Column("Calibrated Range (Min)")]
        public string? CalibratedRangeMin { get; set; }

        [Column("Calibrated Range (Max)")]
        public string? CalibratedRangeMax { get; set; }

        [Column("CR Units")]
        public string? CRUnits { get; set; }

        [Column("Process Range (Min)")]
        public string? ProcessRangeMin { get; set; }

        [Column("Process Range (Max)")]
        public string? ProcessRangeMax { get; set; }

        [Column("PR Units")]
        public string? PRUnits { get; set; }

        [Column("RL / Position")]
        public string? RLPosition { get; set; }

        [Column("Datasheet Number")]
        public string? DatasheetNumber { get; set; }

        [Column("Sheet Number")]
        public string? SheetNumber { get; set; }

        [Column("Hook-up Drawing")]
        public string? HookUpDrawing { get; set; }

        [Column("Termination Diagram")]
        public string? TerminationDiagram { get; set; }

        [Column("P&Id Number")]
        public string? PIDNumber { get; set; }

        [Column("Layout Drawing")]
        public string? LayoutDrawing { get; set; }

        [Column("Architectural Drawing")]
        public string? ArchitecturalDrawing { get; set; }

        [Column("Functional Description Document")]
        public string? FunctionalDescriptionDocument { get; set; }

        [Column("Product Procurement Number")]
        public string? ProductProcurementNumber { get; set; }

        [Column("Junction Box Number")]
        public string? JunctionBoxNumber { get; set; }

        [Column("Nature Of Signal")]
        public string? NatureOfSignal { get; set; }

        [Column("Fail State")]
        public string? FailState { get; set; }

        [Column("GSD Type")]
        public string? GSDType { get; set; }

        [Column("Control Panel Number")]
        public string? ControlPanelNumber { get; set; }

        [Column("PLC Number")]
        public string? PLCNumber { get; set; }

        [Column("PLC Slot Number")]
        public string? PLCSlotNumber { get; set; }

        [Column("Field Panel Number")]
        public string? FieldPanelNumber { get; set; }

        [Column("DP/DP Coupler")]
        public string? DPDPCoupler { get; set; }

        [Column("DP/PA Coupler")]
        public string? DPPACoupler { get; set; }

        [Column("AFD / Hub Number")]
        public string? AFDHubNumber { get; set; }

        [Column("Rack No")]
        public string? RackNo { get; set; }

        [Column("Slot No")]
        public string? SlotNo { get; set; }

        [Column("Channel No")]
        public string? ChannelNo { get; set; }

        [Column("DP Node Address")]
        public string? DPNodeAddress { get; set; }

        [Column("PA Node Address")]
        public string? PANodeAddress { get; set; }

        public int? Revision { get; set; }

        [Column("Revision Changes / Outstanding Comments")]
        public string? RevisionChangesOutstandingComments { get; set; }

        public string? Zone { get; set; }
        public string? Bank { get; set; }
        public string? Service { get; set; }
        public string? Variable { get; set; }
        public string? Train { get; set; }

        [Column("Work Area Pack")]
        public string? WorkAreaPack { get; set; }

        [Column("System Code")]
        public string? SystemCode { get; set; }

        [Column("SubSystem Code")]
        public string? SubsystemCode { get; set; }

        [Column("Historical Logging")]
        public bool? HistoricalLogging { get; set; }

        [Column("Historical Logging Frequency")]
        public double? HistoricalLoggingFrequency { get; set; }

        [Column("Historical Logging Resolution")]
        public double? HistoricalLoggingResolution { get; set; }

        public char? IsInstrument { get; set; }

        public Guid? ProjectId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
