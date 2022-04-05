using System;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace IntegerAI.Trapy.Messages
{

    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa366551(v=vs.85).aspx

    public class TestClass
    {

        public void grap()
        {
            bool _quit = false;
            var message = new byte[1024];

            var messageWait = new EventWaitHandle(false, EventResetMode.AutoReset, Constants.TrapyToCrapy_MemoryMapName);
            var mmf = MemoryMappedFile.CreateOrOpen(Constants.TrapyToCrapy_MemoryMapName, message.Length);
            var viewStream = mmf.CreateViewStream();

            while (!_quit)
            {
                messageWait.WaitOne();
                if (_quit) break;
                viewStream.Position = 0;

                viewStream.Read(message, 0, 4);
                MessageType msg = (MessageType)BitConverter.ToInt32(message, 0);
                switch (msg)
                {
                    case MessageType.ActiveWindowChange:
                        viewStream.Read(message, 4, 300);
                        unsafe
                        {fixed (byte* ptr = &message[0])
                            {
                                ActiveWindowChangedInfo* _Ptr = (ActiveWindowChangedInfo*)ptr;
                                //_Ptr->
                            }
                        }
                        break;
                    case MessageType.KeyBoardAction:
                        break;
                    case MessageType.MouseAction:
                        break;
                    default:
                        break;
                }
                // handle the message
            }
        }
    }
}
