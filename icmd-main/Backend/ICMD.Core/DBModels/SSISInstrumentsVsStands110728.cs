using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SSISInstrumentsVsStands110728 : FullEntityWithAudit<Guid>
    {
        public double? DF { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PLCNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? ProcessNo { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SubProcess { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Stream { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? EquipmentCode { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? SequenceNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? EquipmentIdentifier { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Tag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Function { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Manufacturer { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? DatasheetNumber { get; set; }

        public double? SheetNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? GeneralArrangement { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? TerminationDiagram { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PIDNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? HubNumber { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? VendorSkid { get; set; }

        public double? StandReqd { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? HookupDrawing { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? InstrumentStandTAG { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? InstrumentStandType { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? AncillaryPlate { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Remark { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? OldHOOKUP_172011 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? OldStandTAG_172011 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? OldStandTYPE_172011 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PDMSSTANDQUERYSTANDPRESENT { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? PDMSALLINSTRU { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Working { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Working2 { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? F32 { get; set; }
    }
}
