
namespace Zappy.ZappyActions.Picture.Helpers
{
                public sealed class TextCleanerCropOffset
    {
        internal TextCleanerCropOffset()
        {
            Bottom = 0;
            Left = 0;
            Right = 0;
            Top = 0;
        }

                                public int Bottom
        {
            get;
            set;
        }

                                public int Left
        {
            get;
            set;
        }

                                public int Right
        {
            get;
            set;
        }

                                public int Top
        {
            get;
            set;
        }

        internal bool IsSet
        {
            get
            {
                return Bottom != 0 || Left != 0 || Right != 0 || Top != 0;
            }
        }

        internal bool IsValid
        {
            get
            {
                return Bottom >= 0 && Left >= 0 && Right >= 0 && Top >= 0;
            }
        }
    }
}
