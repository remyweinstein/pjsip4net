namespace pjsip4net.Media
{
    //internal interface ISlotVisitor
    //{
    //    MediaSlotInfo SlotToProcess { get; }
    //    void VisitSlot(MediaSlotInfo slot);
    //    bool ShouldVisit(MediaSlotInfo slot);
    //    Action<MediaSlotInfo, ISlotVisitor> TraverseAlgorythm { get; }
    //}

    //internal class SlotVisitorBase : ISlotVisitor
    //{
    //    private List<int> _visitedIds = new List<int>();

    //    public SlotVisitorBase(MediaSlotInfo slotToProcess, Action<MediaSlotInfo, ISlotVisitor> traverseAlgorythm)
    //    {
    //        Helper.GuardNotNull(slotToProcess);
    //        Helper.GuardNotNull(traverseAlgorythm);
    //        SlotToProcess = slotToProcess;
    //        TraverseAlgorythm = traverseAlgorythm;
    //    }

    //    public virtual void VisitSlot(MediaSlotInfo slot)
    //    {
    //        _visitedIds.Add(slot.Id);
    //        _visitedIds.Sort();
    //    }

    //    public bool ShouldVisit(MediaSlotInfo slot)
    //    {
    //        return _visitedIds.BinarySearch(slot.Id) < 0 && SlotToProcess != slot;
    //    }

    //    public Action<MediaSlotInfo, ISlotVisitor> TraverseAlgorythm { get; private set; }
    //    public MediaSlotInfo SlotToProcess { get; private set; }
    //}

    //internal class ConnectingSlotVisitor : SlotVisitorBase
    //{
    //    public ConnectingSlotVisitor(MediaSlotInfo slotToProcess, Action<MediaSlotInfo, ISlotVisitor> traverseAlgorythm)
    //        : base(slotToProcess, traverseAlgorythm)
    //    {}

    //    public override void VisitSlot(MediaSlotInfo slot)
    //    {
    //        base.VisitSlot(slot);
    //        Helper.GuardError(PJSUA_DLL.Media.pjsua_conf_connect(SlotToProcess.Id, slot.Id));
    //        Helper.GuardError(PJSUA_DLL.Media.pjsua_conf_connect(slot.Id, SlotToProcess.Id));
    //        TraverseAlgorythm(slot, this);
    //    }
    //}

    //internal class DisconnectingSlotVisitor : SlotVisitorBase
    //{
    //    public DisconnectingSlotVisitor(MediaSlotInfo slotToProcess, Action<MediaSlotInfo, ISlotVisitor> traverseAlgorythm)
    //        : base(slotToProcess, traverseAlgorythm)
    //    {
    //    }

    //    public override void VisitSlot(MediaSlotInfo slot)
    //    {
    //        base.VisitSlot(slot);
    //        Helper.GuardError(PJSUA_DLL.Media.pjsua_conf_disconnect(SlotToProcess.Id, slot.Id));
    //        Helper.GuardError(PJSUA_DLL.Media.pjsua_conf_disconnect(slot.Id, SlotToProcess.Id));
    //        TraverseAlgorythm(slot, this);
    //    }
    //}
}