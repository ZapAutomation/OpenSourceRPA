using System.Activities;
using System.ComponentModel;
using System.Linq;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Record
{
    public class StartRecording : TemplateAction
    {
        public DynamicProperty<string> Codec { get; set; } = "MotionJpeg";
        public DynamicProperty<string> Folder { get; set; }
        public DynamicProperty<int> Quality { get; set; } = 70;
        protected void Execute(NativeActivityContext context)
        {
            var strcodec = Codec;
            var folder = Folder;
            var quality = Quality;
            
            if (quality < 10) quality = 10;
            if (quality > 100) quality = 100;
            SharpAvi.FourCC codec;
            if (strcodec == null) strcodec = "motionjpeg";
            switch (strcodec.Value.ToLower())
            {
                case "uncompressed": codec = SharpAvi.KnownFourCCs.Codecs.Uncompressed; break;
                case "motionjpeg": codec = SharpAvi.KnownFourCCs.Codecs.MotionJpeg; break;
                case "microsoftmpeg4v3": codec = SharpAvi.KnownFourCCs.Codecs.MicrosoftMpeg4V3; break;
                case "microsoftmpeg4v2": codec = SharpAvi.KnownFourCCs.Codecs.MicrosoftMpeg4V2; break;
                case "xvid": codec = SharpAvi.KnownFourCCs.Codecs.Xvid; break;
                case "divx": codec = SharpAvi.KnownFourCCs.Codecs.DivX; break;
                case "x264": codec = SharpAvi.KnownFourCCs.Codecs.X264; break;
                default: codec = SharpAvi.KnownFourCCs.Codecs.MotionJpeg; break;
            }
                                                                    }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            throw new System.NotImplementedException();
        }

        public new string DisplayName
        {
            get
            {
                var displayName = base.DisplayName;
                if (displayName == this.GetType().Name)
                {
                    var displayNameAttribute = this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
                    if (displayNameAttribute != null) displayName = displayNameAttribute.DisplayName;
                }
                return displayName;
            }
            set
            {
                base.DisplayName = value;
            }
        }
    }

                                                                                }
