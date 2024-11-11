using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class InstrumentListImport : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ProcessNo { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SubProcess { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(30)]
        public string? Stream { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? EquipmentCode { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? SequenceNumber { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? EquipmentIdentifier { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Tag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ServiceDescription { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Manufacturer { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ModelNumber { get; set; }

        public float? CalibratedRangeMin { get; set; }
        public float? CalibratedRangeMax { get; set; }
        public float? ProcessRangeMin { get; set; }
        public float? ProcessRangeMax { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DataSheetNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SheetNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? HookupDrawing { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? TerminationDiagram { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PIDNumber { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string? NatureOfSignal { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? GSDType { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? ControlPanelNumber { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? PLCNumber { get; set; }
        public int? PLCSlotNumber { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? FieldPanelNumber { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? DPDPCoupler { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? DPPACoupler { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? AFDHubNumber { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? RackNo { get; set; }
        public int? SlotNo { get; set; }
        public int? SlotNoExt { get; set; }
        public int? ChannelNo { get; set; }
        public int? ChannelNoExt { get; set; }
        public int? DPNodeAddress { get; set; }
        public int? PANodeAddress { get; set; }
        public int Revision { get; set; }

        [Column(TypeName = "character varying(1000)")]
        [MaxLength(1000)]
        public string? RevisionChanges { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Service { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Variable { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Train { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Units { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Area { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Bank { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? InstparentTag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Plant { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SubPlantArea { get; set; }

        public bool? VendorSupply { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? SkidNo { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? RLPosition { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? LayoutDrawing { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ArchitectureDrawing { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? JunctionBox { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? FailState { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? InstrumentStand { get; set; }
        public int? WorkPackage { get; set; }
        public int? SystemNo { get; set; }
        public int? SubSystemNo { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? LineVesselNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? FunctionalDescDoc { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ProcurementPkgNum { get; set; }
    }
}
