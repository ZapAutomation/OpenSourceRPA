using System;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;

namespace Zappy.ZappyActions.Record.Helpers
{
    public class Record
    {
        private string outputFolder;
        private FourCC encoder;
        private int encodingQuality;
        private int audioSourceIndex;
        private SupportedWaveFormat audioWaveFormat;
        private bool encodeAudio;
        private int audioQuality;
        public string owner;
        public Record(string owner)
        {
            this.owner = owner;
            InitDefaultSettings();
        }
        private static Record _Instance = null;
        public static Record Instance
        {
            get
            {
                if (_Instance == null) _Instance = new Record(null);
                return _Instance;
            }
        }

        private void InitDefaultSettings()
        {
            
            encoder = KnownFourCCs.Codecs.MotionJpeg;
            encodingQuality = 70;

            audioSourceIndex = -1;
            audioWaveFormat = SupportedWaveFormat.WAVE_FORMAT_44M16;
            encodeAudio = true;
            audioQuality = (Mp3AudioEncoderLame.SupportedBitRates.Length + 1) / 2;
        }

        public bool IsRecording { get; set; }
        public string lastFileName { get; set; }
        private Recorder recorder;
        public void StartRecording(FourCC encoder, string outputFolder, int encodingQuality, string filename = null)
        {
            if (IsRecording)
                throw new InvalidOperationException("Already recording.");

            this.encoder = encoder;
            this.outputFolder = outputFolder;
            this.encodingQuality = encodingQuality;
            string extension = ".avi";
            if (encoder != KnownFourCCs.Codecs.Uncompressed && encoder != KnownFourCCs.Codecs.MotionJpeg) extension = ".mpeg";
            if (!System.IO.Directory.Exists(outputFolder)) System.IO.Directory.CreateDirectory(outputFolder);

                                                                                                                                }

        public void StopRecording()
        {
            if (!IsRecording)
                throw new InvalidOperationException("Not recording.");
                                                                                                                                                                                            }

    }
}
