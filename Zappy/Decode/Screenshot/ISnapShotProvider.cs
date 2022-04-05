using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;

namespace Zappy.Decode.Screenshot
{
    internal interface ISnapShotProvider
    {
        void CleanupImageDirectory();
        ZappyTaskImageInfo TakeAndSaveSnapShotAsync(Screen activeScreen);
    }
}