using System;

namespace ZappyMessages.OutlookMessages
{
    [Serializable]
    public class OutlookMessageInfo
    {
        public string OutlookAccountUserName { get; set; }

        public string SenderEmail { get; set; }

        public string SenderName { get; set; }

        //only doing contains for now
        public string Subject { get; set; }

        //to compare and validate body as well
        public string Body { get; set; }

        public bool OnlySaveLatest { get; set; }

        public string SaveMatchedEmailsDirectory { get; set; }

        public string MoveEmailToFolderNameIfMatched { get; set; }
    }
}