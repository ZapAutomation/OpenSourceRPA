namespace ZappyMessages.OutlookMessages
{
    public interface IOutlookZappyTaskCommunication
    {
        string SearchMessage(OutlookMessageInfo outlookMessageInfo);
        string SearchMessage_OutlookSearch(OutlookMessageInfo messageInfo);
        string RegisterNewEmailTriggerOutlook(OutlookNewEmailTriggerInfo triggerInfo);

    }
}