namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal class ZappyTaskImageInfo
    {
        private string imagePath;
        private long snapShotTimeStamp;

        internal ZappyTaskImageInfo(string path, long snapShotTimeStamp)
        {
            imagePath = path;
            this.snapShotTimeStamp = snapShotTimeStamp;
        }

        internal string ImagePath
        {
            get =>
                imagePath;
            set
            {
                imagePath = value;
            }
        }

        internal long SnapShotTimeStamp =>
            snapShotTimeStamp;
    }
}