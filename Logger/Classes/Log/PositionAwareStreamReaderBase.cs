﻿using System.IO;
using System.Text;
using ZappyLogger.Entities;
using ZappyLogger.Interface;

namespace ZappyLogger.Classes.Log
{
    public abstract class PositionAwareStreamReaderBase : ILogStreamReader
    {
        #region Fields

        protected const int MAX_LINE_LEN = 20000;
        private readonly int _posIncPrecomputed;

        protected readonly int _preambleLength = 0;
        protected readonly StreamReader _reader;
        protected readonly Stream _stream;
        protected int _charBufferPos = 0;

        private Encoding _detectedEncoding;
        protected long _pos;
        protected int _state;

        #endregion

        #region cTor

        protected PositionAwareStreamReaderBase(Stream stream, EncodingOptions encodingOptions)
        {
            _stream = new BufferedStream(stream);
            _preambleLength = DetectPreambleLengthAndEncoding();

            Encoding usedEncoding;
            if (_detectedEncoding != null && encodingOptions.Encoding == null)
            {
                usedEncoding = _detectedEncoding;
            }
            else if (encodingOptions.Encoding != null)
            {
                usedEncoding = encodingOptions.Encoding;
            }
            else
            {
                usedEncoding = encodingOptions.DefaultEncoding != null
                    ? encodingOptions.DefaultEncoding
                    : Encoding.Default;
            }

            if (usedEncoding is UTF8Encoding)
            {
                _posIncPrecomputed = 0;
            }
            else if (usedEncoding is UnicodeEncoding)
            {
                _posIncPrecomputed = 2;
            }
            else
            {
                _posIncPrecomputed = 1;
            }

            _reader = new StreamReader(_stream, usedEncoding, true);
            ResetReader();
            Position = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public long Position
        {
            get { return _pos; }
            set
            {
                /*
                 * 1: Irgendwann mal auskommentiert (+this.Encoding.GetPreamble().Length)
                 * 2: Stand bei 1.1 3207
                 * 3: Stand nach Fehlermeldung von Piet wegen Unicode-Bugs. 
                 *    Keihne Ahnung, ob das jetzt endgültig OK ist.
                 * 4: 27.07.09: Preamble-Length wird jetzt im CT ermittelt, da Encoding.GetPreamble().Length
                 *    immer eine fixe Länge liefert (unabhängig vom echtem Dateiinhalt)
                 */
                _pos = value; //  +this.Encoding.GetPreamble().Length;      // 1
                //this.stream.Seek(this.pos, SeekOrigin.Begin);     // 2
                //this.stream.Seek(this.pos + this.Encoding.GetPreamble().Length, SeekOrigin.Begin);  // 3
                _stream.Seek(_pos + _preambleLength, SeekOrigin.Begin); // 4
                ResetReader();
            }
        }

        public Encoding Encoding
        {
            get { return _reader.CurrentEncoding; }
        }

        public bool IsBufferComplete
        {
            get { return true; }
        }

        #endregion

        #region Public methods

        public void Close()
        {
            _stream.Close();
        }

        public unsafe int ReadChar()
        {
            int readInt;
            try
            {
                readInt = _reader.Read();
                if (readInt != -1)
                {
                    char readChar = (char)readInt;
                    int posInc = _posIncPrecomputed != 0
                        ? _posIncPrecomputed
                        : _reader.CurrentEncoding.GetByteCount(&readChar, 1);
                    _pos += posInc;
                }
            }
            catch (IOException)
            {
                readInt = -1;
            }

            return readInt;
        }

        public abstract string ReadLine();

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the actual number of preamble bytes in the file.
        /// </summary>
        /// <returns></returns>
        private int DetectPreambleLengthAndEncoding()
        {
            /*
            UTF-8:                                EF BB BF 
            UTF-16-Big-Endian-Bytereihenfolge:    FE FF 
            UTF-16-Little-Endian-Bytereihenfolge: FF FE 
            UTF-32-Big-Endian-Bytereihenfolge:    00 00 FE FF 
            UTF-32-Little-Endian-Bytereihenfolge: FF FE 00 00 
            */

            _detectedEncoding = null;
            byte[] readPreamble = new byte[4];
            int readLen = _stream.Read(readPreamble, 0, 4);
            if (readLen < 2)
            {
                return 0;
            }

            Encoding[] encodings = new Encoding[]
            {
                Encoding.UTF8,
                Encoding.Unicode,
                Encoding.BigEndianUnicode,
                Encoding.UTF32
            };
            byte[][] preambles = new byte[4][]
            {
                Encoding.UTF8.GetPreamble(),
                Encoding.Unicode.GetPreamble(),
                Encoding.BigEndianUnicode.GetPreamble(),
                Encoding.UTF32.GetPreamble()
            };
            foreach (Encoding encoding in encodings)
            {
                byte[] preamble = encoding.GetPreamble();
                bool fail = false;
                for (int i = 0; i < readLen && i < preamble.Length; ++i)
                {
                    if (readPreamble[i] != preamble[i])
                    {
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                {
                    _detectedEncoding = encoding;
                    return preamble.Length;
                }
            }

            return 0;
        }

        #endregion

        protected void ResetReader()
        {
            _state = 0;
            NewBuilder();
            _reader.DiscardBufferedData();
        }

        protected void NewBuilder()
        {
            _charBufferPos = 0;
        }
    }
}