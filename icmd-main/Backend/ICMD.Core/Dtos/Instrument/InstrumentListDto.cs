namespace ICMD.Core.Dtos.Instrument
{
    public class InstrumentListDto
    {
        public Guid Id { get; set; }
        public string? ProcessNo { get; set; }
        public string? SubProcess { get; set; }

        public string? Stream { get; set; }

        public string? EquipmentCode { get; set; }

        public string? SequenceNumber { get; set; }

        public string? EquipmentIdentifier { get; set; }

        public string Tag { get; set; }

        public string? ServiceDescription { get; set; }

        public string? Manufacturer { get; set; }

        public string? ModelNumber { get; set; }

        public float? CalibratedRangeMin { get; set; }

        public float? CalibratedRangeMax { get; set; }

        public float? ProcessRangeMin { get; set; }

        public float? ProcessRangeMax { get; set; }

        public string? PIDNumber { get; set; }

        public string? NatureOfSignal { get; set; }

        public string? GSDType { get; set; }

        public string? ControlPanelNumber { get; set; }

        public string? PLCNumber { get; set; }

        public int? PLCSlotNumber { get; set; }

        public string? FieldPanelNumber { get; set; }

        public string? DPDPCoupler { get; set; }

        public string? DPPACoupler { get; set; }

        public string? AFDHubNumber { get; set; }

        public string? RackNo { get; set; }

        public int? SlotNo { get; set; }

        public int? ChannelNo { get; set; }

        public int? DPNodeAddress { get; set; }

        public int? PANodeAddress { get; set; }

        public int Revision { get; set; }

        public string? RevisionChanges { get; set; }

        public string? InstparentTag { get; set; }

        public string? LineVesselNumber { get; set; }

        public string? Plant { get; set; }

        public string? Area { get; set; }

        public bool? VendorSupply { get; set; }

        public string? SkidNo { get; set; }

        public string? RLPosition { get; set; }

        public string? DataSheetNumber { get; set; }

        public string? SheetNumber { get; set; }

        public string? HookupDrawing { get; set; }

        public string? TerminationDiagram { get; set; }

        public string? LayoutDrawing { get; set; }

        public string? ArchitectureDrawing { get; set; }

        public string? JunctionBox { get; set; }

        public string? FailState { get; set; }

        public string? Bank { get; set; }

        public string? Service { get; set; }

        public string? Variable { get; set; }

        public string? Train { get; set; }

        public int? WorkPackage { get; set; }

        public int? SystemNo { get; set; }

        public int? SubSystemNo { get; set; }
        public string? StandNo { get; set; }
        public string? CRUnits { get; set; }
        public string? PRUnits { get; set; }
    }
}
