
using ImageMagick;
using System;

namespace Zappy.ZappyActions.Picture.Helpers
{
                public sealed class TextCleanerScript
    {
                                public TextCleanerScript()
        {
            Reset();
        }

                                        public double AdaptiveBlur
        {
            get;
            set;
        }

                                public MagickColor BackgroundColor
        {
            get;
            set;
        }

                                public TextCleanerCropOffset CropOffset
        {
            get;
            set;
        }

                                public TextCleanerEnhance Enhance
        {
            get;
            set;
        }

                                                public Percentage FilterOffset
        {
            get;
            set;
        }

                                                        public int FilterSize
        {
            get;
            set;
        }

                                public TextCleanerLayout Layout
        {
            get;
            set;
        }

                                public bool MakeGray
        {
            get;
            set;
        }

                                        public int Padding
        {
            get;
            set;
        }

                                        public TextCleanerRotation Rotation
        {
            get;
            set;
        }

                                public Percentage Saturation
        {
            get;
            set;
        }

                                        public double Sharpen
        {
            get;
            set;
        }

                                                public Percentage? SmoothingThreshold
        {
            get;
            set;
        }

                                        public bool Trim
        {
            get;
            set;
        }

                                public bool Unrotate
        {
            get;
            set;
        }

                                                public IMagickImage Execute(IMagickImage input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            CheckSettings();

            var output = input.Clone();
            RotateImage(output);
            CropImage(output);
            ConvertToGrayscale(output);
            EnhanceImage(output);
                        UnrotateImage(output);
            SharpenImage(output);
            SaturateImage(output);
            AdaptiveBlurImage(output);
            TrimImage(output);
            PadImage(output);

            return output;
        }

                                public void Reset()
        {
            AdaptiveBlur = 0.0;
            BackgroundColor = new MagickColor("white");
            CropOffset = new TextCleanerCropOffset();
            Enhance = TextCleanerEnhance.Stretch;
            FilterOffset = (Percentage)5;
            FilterSize = 15;
            Layout = TextCleanerLayout.Portrait;
            MakeGray = false;
            Padding = 0;
            Rotation = TextCleanerRotation.None;
            Saturation = (Percentage)200;
            Sharpen = 0.0;
            SmoothingThreshold = null;
            Trim = false;
            Unrotate = false;
        }

        private void AdaptiveBlurImage(IMagickImage image)
        {
            if (AdaptiveBlur == 0.0)
                return;

            image.AdaptiveBlur(AdaptiveBlur, 1.0);
        }

        private void CheckSettings()
        {
            if (AdaptiveBlur < 0.0)
                throw new InvalidOperationException("Invalid adaptive blur specified, value must be zero or higher.");

            if (!CropOffset.IsValid)
                throw new InvalidOperationException("Invalid crop offset specified, values must be zero or higher.");

            if (FilterSize < 0)
                throw new InvalidOperationException("Invalid filter size specified, value must be zero or higher.");

            if (Padding < 0)
                throw new InvalidOperationException("Invalid padding specified, value must be zero or higher.");

            if (Sharpen < 0.0)
                throw new InvalidOperationException("Invalid sharpen specified, value must be zero or higher.");

            if (Saturation.ToDouble() < 0.0)
                throw new InvalidOperationException("Invalid saturation specified, value must be zero or higher.");

            if (SmoothingThreshold != null)
            {
                double textSmoothingThreshold = SmoothingThreshold.Value.ToDouble();
                if (textSmoothingThreshold < 0 || textSmoothingThreshold > 100)
                    throw new InvalidOperationException("Invalid smoothing threshold specified, value must be between zero and 100.");
            }
        }

        private void ConvertToGrayscale(IMagickImage image)
        {
            if (!MakeGray)
                return;

            image.ColorSpace = ColorSpace.Gray;
            image.ColorType = ColorType.Grayscale;
        }

        private void CropImage(IMagickImage image)
        {
            if (!CropOffset.IsSet)
                return;

            int width = image.Width - (CropOffset.Left + CropOffset.Right);
            int height = image.Height - (CropOffset.Top + CropOffset.Bottom);

            image.Crop(new MagickGeometry(CropOffset.Left, CropOffset.Top, width, height));
        }

        private void EnhanceImage(IMagickImage image)
        {
            if (Enhance == TextCleanerEnhance.Stretch)
                image.ContrastStretch((Percentage)0);
            else if (Enhance == TextCleanerEnhance.Normalize)
                image.Normalize();
        }

        private void PadImage(IMagickImage image)
        {
            if (Padding == 0)
                return;

            image.Compose = CompositeOperator.Over;
            image.BorderColor = BackgroundColor;
            image.Border(Padding);
        }

        private void RemoveNoise(IMagickImage image)
        {
            using (var second = image.Clone())
            {
                second.ColorSpace = ColorSpace.Gray;
                second.Negate();
                second.AdaptiveThreshold(FilterSize, FilterSize, FilterOffset);
                second.ContrastStretch((Percentage)0);

                if (SmoothingThreshold != null)
                {
                    second.Blur(SmoothingThreshold.Value.ToDouble() / 100, Quantum.Max);
                    second.Level(SmoothingThreshold.Value, new Percentage(100));
                }

                image.Composite(second, CompositeOperator.CopyAlpha);
            }

            image.Opaque(MagickColors.Transparent, BackgroundColor);
            image.Alpha(AlphaOption.Off);
        }

        private void RotateImage(IMagickImage image)
        {
            if (Rotation == TextCleanerRotation.None)
                return;
            if ((Layout == TextCleanerLayout.Portrait && image.Height < image.Width) ||
              (Layout == TextCleanerLayout.Landscape && image.Height > image.Width))
            {
                if (Rotation == TextCleanerRotation.Counterclockwise)
                    image.Rotate(90);
                else
                    image.Rotate(-90);
            }
        }

        private void SaturateImage(IMagickImage image)
        {
            if (Saturation == (Percentage)100)
                return;

            image.Modulate((Percentage)100, Saturation, (Percentage)100);
        }

        private void SharpenImage(IMagickImage image)
        {
            if (Sharpen == 0.0)
                return;

            image.Sharpen(0.0, Sharpen);
        }

        private void TrimImage(IMagickImage result)
        {
            if (!Trim)
                return;

            result.Trim();
            result.RePage();
        }

        private void UnrotateImage(IMagickImage image)
        {
            if (!Unrotate)
                return;

            image.BackgroundColor = BackgroundColor;
            image.Deskew((Percentage)40);
        }
    }
}
