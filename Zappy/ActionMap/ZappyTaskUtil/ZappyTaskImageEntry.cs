using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal class ZappyTaskImageEntry : INotifyPropertyChanged
    {
        private ZappyTaskImageInfo imageInfo;
        private Point interactionPoint;

        [field: NonSerialized, CompilerGenerated]
        public event PropertyChangedEventHandler PropertyChanged;

        internal ZappyTaskImageEntry(ZappyTaskImageInfo imageInfo, Point interactionPoint)
        {
            this.imageInfo = imageInfo;
            this.interactionPoint = interactionPoint;
        }

        internal void UpdateZappyTaskImageEntry(ZappyTaskImageInfo imageInformation, Point point)
        {
            imageInfo = imageInformation;
            interactionPoint = point;
            PropertyChanged?.Invoke(this, null);
        }

        internal ZappyTaskImageInfo ImageInformation =>
            imageInfo;

        internal Point InteractionPoint =>
            interactionPoint;
    }
}