using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.ActionMap.TaskAction
{

    internal interface IZappyActionStack
    {
        ZappyTaskAction Peek();
        ZappyTaskAction Peek(int nth);
        ZappyTaskAction Pop();
        ZappyTaskAction Pop(int nth);
        void Push(ZappyTaskAction element);

        int Count { get; }

        object SyncRoot { get; }
    }
}
